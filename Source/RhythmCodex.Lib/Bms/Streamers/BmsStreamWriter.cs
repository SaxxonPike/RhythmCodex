using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Streamers
{
    [Service]
    public class BmsStreamWriter : IBmsStreamWriter
    {
        public void Write(Stream stream, IEnumerable<string> commands)
        {
            var writer = new StreamWriter(stream) {AutoFlush = false};
            foreach (var command in commands)
                writer.WriteLine(command);
            writer.Flush();
        }
    }
}