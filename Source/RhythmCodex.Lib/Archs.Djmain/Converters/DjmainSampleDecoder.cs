using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Archs.Djmain.Streamers;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Djmain.Converters;

[Service]
public class DjmainSampleDecoder(
    IDjmainAudioStreamReader djmainAudioStreamReader)
    : IDjmainSampleDecoder
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
        foreach (var (key, props) in infos.OrderBy(x => x.Key))
        {
            if (props.Frequency == 0)
                continue;
            
            // This is a failsafe for data reaching the end of a sample table.
            if (props is { Frequency: 0x0A0A, Channel: 0x0A, Panning: 0x0A, Volume: 0x0A })
                continue;

            // A likewise problem with 0x4F.
            if (props is { Frequency: 0x4F4F, Channel: 0x4F, Panning: 0x4F, Volume: 0x4F })
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