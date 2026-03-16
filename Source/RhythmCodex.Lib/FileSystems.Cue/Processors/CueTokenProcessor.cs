using System.Collections.Generic;
using System.Text;
using RhythmCodex.FileSystems.Cue.Helpers;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Cue.Processors;

/// <inheritdoc />
[Service]
public sealed class CueTokenProcessor : ICueTokenProcessor
{
    /// <inheritdoc />
    public List<string> ProcessTokens(string line, StringBuilder? sb = null)
    {
        //
        // Short circuit for blank lines.
        //

        if (string.IsNullOrWhiteSpace(line))
            return [];

        //
        // Parse individual tokens in a CUE line.
        //

        var quote = false;
        var result = new List<string>();
        sb.Clear();

        //
        // Process each character.
        //

        foreach (var c in line)
        {
            switch (c, quote)
            {
                case ('"', false):
                {
                    // Starting quote.

                    AddToken();
                    quote = true;
                    continue;
                }
                case ('"', true):
                {
                    // Ending quote.

                    AddToken();
                    quote = false;
                    continue;
                }
                case (_, true):
                {
                    // Any character between quotes.

                    break;
                }
                case (_, _) when char.IsWhiteSpace(c):
                {
                    // Whitespace separates tokens.

                    AddToken();
                    continue;
                }
            }

            sb.Append(c);
        }

        //
        // If whitespace is not present at the end of the line,
        // commit any remainder of a token in the buffer.
        //

        AddToken();

        return result;

        //
        // Invoke this to commit tokens to the result.
        //

        void AddToken()
        {
            if (sb.Length > 0)
                result.Add(sb.ToString());
            sb.Clear();
        }
    }

    /// <inheritdoc />
    public int? ConvertMsfTimeToSector(string value)
    {
        var match = CueRegex.MinutesSecondsFrames().Match(value);

        if (!match.Success)
            return null;

        var minutes = int.Parse(match.Groups[1].ValueSpan);
        var seconds = int.Parse(match.Groups[2].ValueSpan);
        var frames = int.Parse(match.Groups[3].ValueSpan);

        return (minutes * 60 + seconds) * 75 + frames;
    }
}