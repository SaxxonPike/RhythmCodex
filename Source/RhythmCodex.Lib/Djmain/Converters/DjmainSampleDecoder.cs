using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Converters;

[Service]
public class DjmainSampleDecoder(IDjmainAudioStreamReader djmainAudioStreamReader) : IDjmainSampleDecoder
{
    public Dictionary<int, DjmainSample> Decode(
        Stream stream,
        IEnumerable<KeyValuePair<int, DjmainSampleInfo>> infos,
        int sampleOffset)
    {
        return DecodeInternal(stream, infos, sampleOffset)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private IEnumerable<KeyValuePair<int, DjmainSample>> DecodeInternal(
        Stream stream,
        IEnumerable<KeyValuePair<int, DjmainSampleInfo>> infos,
        int sampleOffset)
    {
        foreach (var (key, props) in infos)
        {
            if (props.Frequency == 0)
                continue;
                
            // There's fuckery in some of the samples. This addresses a specific anomaly in the input
            // data where the whole line is 0x0A repeated.
            if (props is { Frequency: 0x0A0A, Channel: 0x0A, Panning: 0x0A, Volume: 0x0A })
                continue;

            stream.Position = sampleOffset + props.Offset;

            yield return new KeyValuePair<int, DjmainSample>(key,
                new DjmainSample
                {
                    Data = GetSampleData(),
                    Info = props
                });

            continue;

            Memory<byte> GetSampleData() =>
                (props.SampleType & 0xC) switch
                {
                    0x0 => djmainAudioStreamReader.ReadPcm8(stream),
                    0x4 => djmainAudioStreamReader.ReadPcm16(stream),
                    0x8 => djmainAudioStreamReader.ReadDpcm(stream),
                    _ => Memory<byte>.Empty
                };
        }
    }
}