using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RhythmCodex.Bms.Model;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Streamers
{
    [Service]
    public class BmsStreamReader : IBmsStreamReader
    {
        public IEnumerable<BmsCommand> Read(Stream stream)
        {
            var text = stream.ReadAllText();
            
            // Remove all comments.
            text = Regex.Replace(text, @"\/\*(\*(?!\/)|[^*])*\*\/", string.Empty); // remove /* */
            text = Regex.Replace(text, @"(\/\/|\;).*$", string.Empty, RegexOptions.Multiline); // remove // and ;
            
            // Parse remaining lines.
            return ConvertLines(text.SplitLines()).ToArray();
        }

        private IEnumerable<BmsCommand> ConvertLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine))
                    continue;
                
                // Lines not starting with # are considered comments.
                if (trimmedLine[0] != '#')
                {
                    yield return new BmsCommand
                    {
                        Comment = trimmedLine
                    };
                    continue;
                }

                // Determine delimiter.
                var cmd = new BmsCommand();
                var colonIndex = trimmedLine.IndexOf(':');
                var spaceIndex = trimmedLine.IndexOf(' ');
                var delimiterIndex = -1;

                if (colonIndex >= 0 && spaceIndex >= 0)
                    delimiterIndex = Math.Min(colonIndex, spaceIndex);
                else if (colonIndex >= 0)
                    delimiterIndex = colonIndex;
                else if (spaceIndex >= 0)
                    delimiterIndex = spaceIndex;

                if (delimiterIndex >= 0)
                {
                    cmd.Name = trimmedLine.Substring(1, delimiterIndex - 1).Trim();
                    cmd.Value = trimmedLine.Substring(delimiterIndex + 1).Trim();
                    if (delimiterIndex == colonIndex)
                        cmd.UseColon = true;
                }
                else
                {
                    cmd.Name = trimmedLine.Substring(1);
                }

                yield return cmd;
            }
        }
    }
}