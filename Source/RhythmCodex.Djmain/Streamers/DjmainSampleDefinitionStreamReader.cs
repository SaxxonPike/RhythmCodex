using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    public class DjmainSampleDefinitionStreamReader : IDjmainSampleDefinitionStreamReader
    {
        public IDictionary<int, DjmainSampleDefinition> Read(Stream stream)
        {
            return ReadInternal(stream).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private static IEnumerable<KeyValuePair<int, DjmainSampleDefinition>> ReadInternal(Stream stream)
        {
            var buffer = new byte[11];

            using (var mem = new MemoryStream(buffer))
            {
                var reader = new BinaryReader(mem);

                for (var i = 0; i < DjmainConstants.MaxSampleDefinitions; i++)
                {
                    reader.BaseStream.Position = 0;

                    var bytesRead = stream.Read(buffer, 0, 11);
                    if (bytesRead < 11)
                        yield break;

                    var result = new DjmainSampleDefinition
                    {
                        Channel = reader.ReadByte(),
                        Frequency = reader.ReadUInt16(),
                        ReverbVolume = reader.ReadByte(),
                        Volume = reader.ReadByte(),
                        Panning = reader.ReadByte(),
                        Offset = reader.ReadUInt16() | ((uint)reader.ReadByte() << 16),
                        SampleType = reader.ReadByte(),
                        Flags = reader.ReadByte()
                    };
                    yield return new KeyValuePair<int, DjmainSampleDefinition>(i, result);

                }
            }
        }
    }
}
