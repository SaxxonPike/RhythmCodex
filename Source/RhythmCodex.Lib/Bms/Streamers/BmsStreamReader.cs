using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RhythmCodex.Bms.Model;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Bms.Streamers;

[Service]
public partial class BmsStreamReader : IBmsStreamReader
{
    private const string Delimiters = " \t:";

    public IEnumerable<BmsCommand> Read(Stream stream)
    {
        var text = stream.ReadAllText();

        // Remove all comments.
        text = StarCommentRegex().Replace(text, string.Empty);
        text = SlashSemiColonRegex().Replace(text, string.Empty);

        // Parse remaining lines.
        return ConvertLines(text.SplitLines()).ToArray();
    }

    private static IEnumerable<BmsCommand> ConvertLines(IEnumerable<string> lines)
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
            var defaultDelimiter = (Delimiter: '\0', Index: -1);
            var cmd = new BmsCommand();
            var delimiterOffset = Delimiters
                .Select(d => (Delimiter: d, Index: trimmedLine.IndexOf(d)))
                .Where(kv => kv.Index >= 0)
                .DefaultIfEmpty(defaultDelimiter)
                .MinBy(kv => kv.Index);

            if (delimiterOffset.Index >= 0)
            {
                cmd.Name = trimmedLine[1..delimiterOffset.Index]
                    .Trim()
                    .ToUpperInvariant();

                cmd.Value = trimmedLine[(delimiterOffset.Index + 1)..]
                    .Trim();

                if (delimiterOffset.Delimiter == ':')
                    cmd.UseColon = true;
            }
            else
            {
                cmd.Name = trimmedLine[1..].ToUpperInvariant();
            }

            yield return cmd;
        }
    }

    /// <summary>
    /// Remove /* and */ comments.
    /// </summary>
    [GeneratedRegex(@"\/\*(\*(?!\/)|[^*])*\*\/")]
    private static partial Regex StarCommentRegex();

    /// <summary>
    /// Remove // and ; comments.
    /// </summary>
    [GeneratedRegex(@"(\/\/|\;).*$", RegexOptions.Multiline)]
    private static partial Regex SlashSemiColonRegex();
}