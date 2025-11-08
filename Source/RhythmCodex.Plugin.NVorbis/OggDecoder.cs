using System;
using System.IO;
using System.Runtime.InteropServices;
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
        
        var result = audioDsp.FloatsToSound(rawData, channels);
        if (result == null)
            return null;

        result[NumericData.Rate] = rate;
        return result;
    }
}