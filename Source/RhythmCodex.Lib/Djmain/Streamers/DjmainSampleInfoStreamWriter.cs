using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Streamers;

[Service]
public class DjmainSampleInfoStreamWriter(IDjmainConfiguration djmainConfiguration)
    : IDjmainSampleInfoStreamWriter
{
    public void Write(Stream stream, IReadOnlyCollection<KeyValuePair<int, DjmainSampleInfo>> definitions)
    {
        var count = Math.Max(0,
            Math.Min(definitions.Any() ? definitions.Max(d => d.Key) + 1 : 0, djmainConfiguration.MaxSampleDefinitions));

        var writer = new BinaryWriter(stream);

        for (var i = 0; i < count; i++)
        {
            var definition = definitions.SingleOrDefault(d => d.Key == i).Value;
            writer.Write(definition.Channel);
            writer.Write(definition.Frequency);
            writer.Write(definition.ReverbVolume);
            writer.Write(definition.Volume);
            writer.Write(definition.Panning);
            writer.Write(unchecked((ushort) definition.Offset));
            writer.Write(unchecked((byte) (definition.Offset >> 16)));
            writer.Write(definition.SampleType);
            writer.Write(definition.Flags);
        }
    }
}