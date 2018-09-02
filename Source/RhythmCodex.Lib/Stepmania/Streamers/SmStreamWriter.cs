using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Streamers
{
    [Service]
    public class SmStreamWriter : ISmStreamWriter
    {
        public void Write(Stream stream, IEnumerable<Command> commands)
        {
            var writer = new StreamWriter(stream);
            writer.WriteLine($"// RhythmCodex {DateTime.Now:s}");
            foreach (var command in commands)
                writer.WriteLine($"#{command.Name.ToUpperInvariant()}:{string.Join(":", command.Values)};");
            writer.Flush();
        }
    }
}