using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Plugin.Sdl3.Infrastructure;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Streamers;
using SDL;
using static SDL.SDL3;
using static SDL.SDL3_mixer;

namespace RhythmCodex.Plugin.Sdl3.Sounds;

[Service]
public class SoundStreamReader : ISoundStreamReader
{
    public Sound Read(Stream stream)
    {
        Init.EnsureLoaded();

        unsafe
        {
            MIX_AudioDecoder* decoder = null;

            using var io = new IoStreamWrapper(stream);

            try
            {
                //
                // First, open the decoder on the stream.
                //

                decoder = MIX_CreateAudioDecoder_IO((SDL_IOStream*)io.SdlIoStream, false, 0);
                if (decoder == null)
                    throw new SdlException();

                //
                // Tweak the format so that we can use the data directly (float).
                //

                SDL_AudioSpec spec;
                if (!MIX_GetAudioDecoderFormat(decoder, &spec))
                    throw new SdlException();

                if (spec.channels < 1 || spec.freq <= 0)
                    throw new RhythmCodexException("Incompatible audio format.");

                spec.format = SDL_AUDIO_F32;

                //
                // Begin decoding the audio. We allocate a small (16k per channel) buffer to pipe the data.
                //

                var channel = 0;
                var samples = new List<List<float>>();

                for (var i = 0; i < spec.channels; i++)
                    samples.Add([]);
                
                Span<float> buffer = stackalloc float[4096 * spec.channels];
                var bufferSizeBytes = sizeof(float) * buffer.Length;

                fixed (void* bufferPtr = buffer)
                {
                    while (true)
                    {
                        var actualRead = MIX_DecodeAudio(decoder, (IntPtr)bufferPtr, bufferSizeBytes, &spec) /
                                         sizeof(float);

                        if (actualRead < 1)
                            break;

                        for (var i = 0; i < actualRead; i++)
                        {
                            samples[channel++].Add(buffer[i]);
                            if (channel >= spec.channels)
                                channel = 0;
                        }
                    }
                }
                
                //
                // With the audio data decoded and deinterleaved, build the result.
                //

                var result = new Sound
                {
                    [NumericData.Rate] = spec.freq
                };

                for (var i = 0; i < spec.channels; i++)
                {
                    result.Samples.Add(new Sample
                    {
                        Data = samples[i].ToArray()
                    });
                }

                return result;
            }
            finally
            {
                if (decoder != null)
                    MIX_DestroyAudioDecoder(decoder);
            }
        }
    }
}