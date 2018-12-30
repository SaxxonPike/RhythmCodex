using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Streamers
{
    [Service]
    public class SmStreamReader : ISmStreamReader
    {
        public IEnumerable<Command> Read(Stream source)
        {
            var lines = source.ReadAllLines();
            var commands = ExtractCommands(lines);

            return commands
                .Select(c => c.Split(':'))
                .Select(cl => new Command {Name = cl.First(), Values = cl.Skip(1).ToArray()});
        }

        private static IEnumerable<string> ExtractCommands(IEnumerable<string> lines)
        {
            var commandMode = false;
            var commandBuilder = new StringBuilder();

            foreach (var line in lines)
            {
                foreach (var c in line)
                    if (!commandMode)
                    {
                        if (c == '#')
                            commandMode = true;
                    }
                    else
                    {
                        if (c == ';')
                        {
                            commandMode = false;
                            if (commandBuilder.Length > 0)
                            {
                                yield return commandBuilder.ToString();
                                commandBuilder.Clear();
                            }
                            continue;
                        }

                        commandBuilder.Append(c);
                    }

                if (commandBuilder.Length > 0)
                    commandBuilder.AppendLine();
            }

            if (commandBuilder.Length > 0)
                yield return commandBuilder.ToString();
        }
    }
}