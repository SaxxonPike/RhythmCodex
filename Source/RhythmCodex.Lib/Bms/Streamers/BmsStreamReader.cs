using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RhythmCodex.Bms.Model;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Bms.Streamers
{
    [Service]
    public class BmsStreamReader : IBmsStreamReader
    {
        private static readonly char[] Delimiters = {' ', '\t', ':'};
        
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
                var delimiterOffset = Delimiters
                    .Select(d => new {Delimiter = d, Index = trimmedLine.IndexOf(d)})
                    .Where(kv => kv.Index >= 0)
                    .OrderBy(kv => kv.Index)
                    .FirstOrDefault();

                if (delimiterOffset != null)
                {
                    cmd.Name = trimmedLine.Substring(1, delimiterOffset.Index - 1).Trim().ToUpperInvariant();
                    cmd.Value = trimmedLine.Substring(delimiterOffset.Index + 1).Trim();
                    if (delimiterOffset.Delimiter == ':')
                        cmd.UseColon = true;
                }
                else
                {
                    cmd.Name = trimmedLine.Substring(1).ToUpperInvariant();
                }
                
                yield return cmd;
            }
        }
    }
}