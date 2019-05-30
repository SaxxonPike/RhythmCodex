using System.Collections.Generic;
using System.IO;
using System.Text;
using RhythmCodex.Bms.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Bms.Streamers
{
    [Service]
    public class BmsStreamWriter : IBmsStreamWriter
    {
        public void Write(Stream stream, IEnumerable<BmsCommand> commands)
        {
            var writer = new StreamWriter(stream) {AutoFlush = false};
            foreach (var command in commands)
            {
                var line = new StringBuilder();
                if (!string.IsNullOrEmpty(command.Name))
                {
                    line.Append($"#{command.Name}");
                    if (!string.IsNullOrEmpty(command.Value))
                        line.Append($"{(command.UseColon ? ":" : " ")}{command.Value}");
                }

                if (!string.IsNullOrEmpty(command.Comment))
                {
                    if (line.Length > 0)
                        line.Append(" ;");
                    line.Append($"{command.Comment}");
                }

                if (line.Length > 0)
                    writer.WriteLine(line);
            }

            writer.Flush();
        }
    }
}