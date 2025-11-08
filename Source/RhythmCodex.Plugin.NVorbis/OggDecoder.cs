using System.IO;
using System.Linq;
using NVorbis;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Ogg.Converters;

namespace RhythmCodex.Plugin.NVorbis;

[Service]
public class OggDecoder(IAudioDsp audioDsp) : IOggDecoder
{
    public Sound? Decode(Stream stream)
    {
        using var reader = new VorbisReader(stream, false);
        reader.ClipSamples = false;
        var channels = reader.Channels;
        var rate = reader.SampleRate;

        var rawData = new float[reader.TotalSamples];
        reader.ReadSamples(rawData);

        var result = audioDsp.FloatsToSamples(rawData, channels);

        foreach (var sample in result)
            sample[NumericData.Rate] = rate;

        return new Sound
        {
            Samples = result.ToList(),
            [NumericData.Rate] = rate
        };
    }
}