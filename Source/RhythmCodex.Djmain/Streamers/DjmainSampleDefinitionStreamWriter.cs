﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Extensions;

namespace RhythmCodex.Djmain.Streamers
{
    public class DjmainSampleDefinitionStreamWriter : IDjmainSampleDefinitionStreamWriter
    {
        public void Write(Stream stream, IEnumerable<KeyValuePair<int, DjmainSampleDefinition>> definitions)
        {
            var defs = definitions.AsList();
            var count = Math.Max(0, Math.Min(defs.Any() ? defs.Max(d => d.Key) + 1 : 0, DjmainConstants.MaxSampleDefinitions));

            var writer = new BinaryWriter(stream);

            for (var i = 0; i < count; i++)
            {
                var definition = defs.SingleOrDefault(d => d.Key == i).Value;
                writer.Write(definition.Channel);
                writer.Write(definition.Frequency);
                writer.Write(definition.ReverbVolume);
                writer.Write(definition.Volume);
                writer.Write(definition.Panning);
                writer.Write(unchecked((ushort)definition.Offset));
                writer.Write(unchecked((byte)(definition.Offset >> 16)));
                writer.Write(definition.SampleType);
                writer.Write(definition.Flags);
            }
        }
    }
}
