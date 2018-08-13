using System.Collections.Generic;
using System.Linq;
using System.Text;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Extensions
{
    /// <summary>
    ///     Extensions to the .NET string class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Split a string based on a delimiter, enumerating output as opposed to the standard .Split which will
        ///     buffer all contents in memory first.
        /// </summary>
        public static IEnumerable<string> SplitEx(this string input, char delimiter)
        {
            var buffer = new StringBuilder();
            foreach (var c in input)
            {
                if (c == delimiter)
                {
                    yield return buffer.ToString();
                    buffer.Clear();
                    continue;
                }

                buffer.Append(c);
            }
        }

        /// <summary>
        ///     Split a string up into lines. The delimiters are 0D+0A, 0A, and 0D.
        /// </summary>
        public static IEnumerable<string> SplitLines(this string input)
        {
            var buffer = new StringBuilder();
            var b0 = input[0];
            var b1 = '\0';
            var skip = false;

            foreach (var c in input.Skip(1))
            {
                b1 = c;
                if (!skip)
                    switch (b0)
                    {
                        case '\xa':
                            yield return buffer.ToString();
                            buffer.Clear();
                            break;
                        case '\xd':
                            yield return buffer.ToString();
                            buffer.Clear();

                            if (b1 == '\xa')
                                skip = true;
                            break;
                        default:
                            buffer.Append(b0);
                            break;
                    }
                else
                    skip = false;
                b0 = b1;
            }

            if (!skip && (b1 == '\xa' || b1 == '\xd'))
            {
                yield return buffer.ToString();
                buffer.Clear();
            }
            else
            {
                buffer.Append(b1);
            }

            yield return buffer.ToString();
        }

        /// <summary>
        /// Convert a string to bytes using Codepage 932.
        /// </summary>
        public static byte[] GetShiftJisBytes(this string s) 
            => Encodings.CP932.GetBytes(s);
        
        /// <summary>
        /// Convert a string from bytes using Codepage 932.
        /// </summary>
        public static string GetShiftJisString(this byte[] b) 
            => Encodings.CP932.GetString(b);
        
        /// <summary>
        /// Convert a string to bytes using Codepage 437.
        /// </summary>
        public static byte[] GetBytes(this string s) 
            => Encodings.CP437.GetBytes(s);
        
        /// <summary>
        /// Convert a string from bytes using Codepage 437.
        /// </summary>
        public static string GetString(this byte[] b) 
            => Encodings.CP437.GetString(b);
    }
}