using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    public class DjmainSampleDefinitionStreamWriter : IDjmainSampleDefinitionStreamWriter
    {
        public void Write(Stream stream, IEnumerable<DjmainSampleDefinition> definitions)
        {
            var writer = new BinaryWriter(stream);

            foreach (var definition in definitions.Take(DjmainConstants.MaxSampleDefinitions))
            {
                writer.Write(definition.Channel);
                writer.Write(definition.Frequency);
                writer.Write(definition.ReverbVolume);
                writer.Write(definition.Volume);
                writer.Write(definition.Panning);
                writer.Write(unchecked((ushort)definition.Offset));
                writer.Write(unchecked((byte)definition.Offset >> 16));
                writer.Write(definition.SampleType);
                writer.Write(definition.Flags);
            }
        }
    }
}
