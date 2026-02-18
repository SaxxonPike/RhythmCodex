using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Firebeat.Models;
using RhythmCodex.Games.Beatmania.Converters;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Firebeat.Converters;

[Service]
public class FirebeatSoundDecoder(
    IBeatmaniaDspTranslator beatmaniaDspTranslator)
    : IFirebeatSoundDecoder
{
    public ReadOnlySpan<byte> TrimAudio(ReadOnlySpan<byte> data)
    {
        for (var j = data.Length - 4; j >= 0; j -= 4)
        {
            var val = ReadUInt32LittleEndian(data[j..]);

            if (val is 0x80008000u or 0x00800080u or 0x0A0A0A0Au or 0x00000000u)
                continue;

            return data[..j];
        }

        return data;
    }


    public Dictionary<int, Sound> Decode(
        IEnumerable<KeyValuePair<int, FirebeatSample>> samples
    )
    {
        return samples.ToDictionary(
            x => x.Key,
            x =>
            {
                var info = x.Value.Info;
                var data = TrimAudio(x.Value.Data.Span);
                var isStereo = (info.Flag0F & FirebeatSampleFlag0F.Stereo) != 0;
                var resultSamples = new List<Sample>();

                //
                // Allocate a temporary buffer for sample data converted to float.
                //

                var inByteCount = data.Length & ~1;
                var inWordCount = inByteCount / 2;
                using var inFloats = MemoryPool<float>.Shared.Rent(inWordCount);
                var inFloatsSpan = inFloats.Memory.Span[..inWordCount];

                for (int i = 0, j = 0; i < inByteCount; i += 2, j++)
                {
                    var val = ReadUInt16BigEndian(data[i..]);
                    inFloatsSpan[j] = ((val & 0x8000) == 0 ? val : -(val ^ 0x8000)) / 32768f;
                }

                //
                // Stereo samples need to be deinterleaved. Mono samples will be
                // doubled to stereo.
                //

                if (!isStereo)
                {
                    resultSamples.AddRange([
                        new Sample
                        {
                            Data = inFloatsSpan.ToArray()
                        },
                        new Sample
                        {
                            Data = inFloatsSpan.ToArray()
                        }
                    ]);
                }
                else
                {
                    var outLength = inWordCount / 2;
                    var outFloatsL = new float[outLength];
                    var outFloatsR = new float[outLength];

                    AudioSimd.Deinterleave2(inFloatsSpan, outFloatsL, outFloatsR);

                    resultSamples.AddRange([
                        new Sample
                        {
                            Data = outFloatsL
                        },
                        new Sample
                        {
                            Data = outFloatsR
                        }
                    ]);
                }

                //
                // Set sound parameters from info.
                //

                return new Sound
                {
                    Samples = resultSamples,
                    [NumericData.Rate] = info.Frequency,
                    [NumericData.SourceRate] = info.Frequency,
                    [NumericData.Volume] = beatmaniaDspTranslator.GetFirebeatVolume(info.Volume),
                    [NumericData.SourceVolume] = info.Volume,
                    [NumericData.Panning] = Math.Clamp(info.Panning, (byte)0x00, (byte)0x7F) / 127f,
                    [NumericData.SourcePanning] = info.Panning,
                    [NumericData.Channel] = info.Channel < 0xFF ? info.Channel : null,
                    [NumericData.SampleMap] = 0,
                    [NumericData.Id] = x.Key
                };
            });
    }
}