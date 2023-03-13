using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Converters;

[Service]
public class DjmainSampleDecoder : IDjmainSampleDecoder
{
    private readonly IDjmainAudioStreamReader _djmainAudioStreamReader;

    public DjmainSampleDecoder(
        IDjmainAudioStreamReader djmainAudioStreamReader)
    {
        _djmainAudioStreamReader = djmainAudioStreamReader;
    }

    public IDictionary<int, IDjmainSample> Decode(
        Stream stream,
        IEnumerable<KeyValuePair<int, IDjmainSampleInfo>> infos,
        int sampleOffset)
    {
        return DecodeInternal(stream, infos, sampleOffset)
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    private IEnumerable<KeyValuePair<int, IDjmainSample>> DecodeInternal(
        Stream stream,
        IEnumerable<KeyValuePair<int, IDjmainSampleInfo>> infos,
        int sampleOffset)
    {
        foreach (var info in infos)
        {
            var props = info.Value;

            if (props.Frequency == 0)
                continue;
                
            // There's fuckery in some of the samples. This addresses a specific anomaly in the input
            // data where the whole line is 0x0A repeated.
            if (props.Frequency == 0x0A0A &&
                props.Channel == 0x0A &&
                props.Panning == 0x0A &&
                props.Volume == 0x0A)
                continue;

            IList<byte> GetSampleData()
            {
                switch (props.SampleType & 0xC)
                {
                    case 0x0:
                        return _djmainAudioStreamReader.ReadPcm8(stream);
                    case 0x4:
                        return _djmainAudioStreamReader.ReadPcm16(stream);
                    case 0x8:
                        return _djmainAudioStreamReader.ReadDpcm(stream);
                    default:
                        return new List<byte>();
                }
            }

            stream.Position = sampleOffset + props.Offset;

            yield return new KeyValuePair<int, IDjmainSample>(info.Key,
                new DjmainSample
                {
                    Data = GetSampleData(),
                    Info = props
                });
        }
    }
}