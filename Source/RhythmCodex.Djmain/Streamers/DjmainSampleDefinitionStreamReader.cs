using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    public class DjmainSampleDefinitionStreamReader : IDjmainSampleDefinitionStreamReader
    {
        private const int MaxDefinitionCount = 256;
        
        public IEnumerable<DjmainSampleDefinition> Read(Stream stream)
        {
            var reader = new BinaryReader(stream);

            for (var i = 0; i < MaxDefinitionCount; i++)
            {
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
                
                yield return result;
            }
        }
    }
}
