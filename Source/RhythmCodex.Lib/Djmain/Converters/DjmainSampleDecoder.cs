using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Converters
{
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
            byte[] data,
            IEnumerable<KeyValuePair<int, IDjmainSampleInfo>> infos,
            int sampleOffset)
        {
            return DecodeInternal(data, infos, sampleOffset)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private IEnumerable<KeyValuePair<int, IDjmainSample>> DecodeInternal(
            byte[] data,
            IEnumerable<KeyValuePair<int, IDjmainSampleInfo>> infos,
            int sampleOffset)
        {
            using (var mem = new MemoryStream(data))
            {
                foreach (var info in infos)
                {
                    var props = info.Value;

                    IList<byte> GetSampleData()
                    {
                        switch (props.SampleType & 0xC)
                        {
                            case 0x0:
                                return _djmainAudioStreamReader.ReadPcm8(mem);
                            case 0x4:
                                return _djmainAudioStreamReader.ReadPcm16(mem);
                            case 0x8:
                                return _djmainAudioStreamReader.ReadDpcm(mem);
                            default:
                                return new List<byte>();
                        }
                    }

                    mem.Position = sampleOffset + props.Offset;

                    yield return new KeyValuePair<int, IDjmainSample>(info.Key,
                        new DjmainSample
                        {
                            Data = GetSampleData(),
                            Info = props
                        });
                }
            }
        }
    }
}