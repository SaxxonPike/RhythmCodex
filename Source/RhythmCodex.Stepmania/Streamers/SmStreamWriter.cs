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
            foreach (var command in commands)
                writer.WriteLine($"#{command.Name}:{string.Join(":", command.Values)};");
            writer.Flush();
        }
    }
}
