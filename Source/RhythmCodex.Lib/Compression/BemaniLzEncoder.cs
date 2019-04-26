using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression
{
    [Service]
    public class BemaniLzEncoder : IBemaniLzEncoder
    {
        private enum TokenType
        {
            Raw,
            Short,
            Long
        }

        private struct Match
        {
            public int Offset;
            public int Length;
        }

        private class Token
        {
            public int Offset { get; set; }
            public int Length { get; set; }
            public TokenType Type { get; set; }
            public List<byte> Data { get; } = new List<byte>();
        }

        /// <summary>
        /// Read the source data and determine how to compress it.
        /// </summary>
        private static IEnumerable<Token> GetTokens(IReadOnlyList<byte> source)
        {
            // Long:  0LLLLLDD DDDDDDDD (length 3-34, distance 0-1023)
            // Short: 10LLDDDD (length 2-5, distance 1-16)
            // Block: 11LLLLLL (length 7-69, 1:1)

            Match FindMatch(int lowIndex, int cursorIndex, int length,
                int matchMinLength, int matchMaxLength)
            {
                var bestOffset = -1;
                var bestLength = matchMinLength;

                for (var idx = Math.Max(0, lowIndex); idx < cursorIndex; idx++)
                {
                    var match = true;
                    var matchLength = 0;

                    for (var matchIdx = 0;
                        matchIdx < matchMaxLength && matchIdx + idx < length && cursorIndex + matchIdx < length;
                        matchIdx++)
                    {
                        if (source[idx + matchIdx] != source[cursorIndex + matchIdx])
                        {
                            match = false;
                            break;
                        }

                        matchLength++;
                    }

                    if (match && matchLength >= bestLength)
                    {
                        bestLength = matchLength;
                        bestOffset = idx;
                    }
                }

                return new Match
                {
                    Offset = bestOffset,
                    Length = bestOffset < 0 ? 0 : bestLength
                };
            }

            var inputLength = source.Count;
            var inputOffset = 0;

            var tokens = new List<Token>();
            Token token = null;
            while (inputOffset < inputLength)
            {
                var shortMatch = FindMatch(inputOffset - 0x10, inputOffset, inputLength, 2, 4);
                var longMatch = FindMatch(inputOffset - 0x3FF, inputOffset, inputLength, 3, 33);

                var useShortMatch = shortMatch.Offset >= 0 &&
                                    shortMatch.Length > 0 &&
                                    shortMatch.Length > longMatch.Length;
                var useLongMatch = longMatch.Offset >= 0 &&
                                   longMatch.Length > 0 &&
                                   longMatch.Length >= shortMatch.Length;

                if (useShortMatch)
                {
                    if (token != null)
                        tokens.Add(token);
                    token = null;

                    tokens.Add(new Token
                    {
                        Offset = inputOffset - shortMatch.Offset,
                        Length = shortMatch.Length,
                        Type = TokenType.Short
                    });

                    inputOffset += shortMatch.Length;
                }
                else if (useLongMatch)
                {
                    if (token != null)
                        tokens.Add(token);
                    token = null;

                    tokens.Add(new Token
                    {
                        Offset = inputOffset - longMatch.Offset,
                        Length = longMatch.Length,
                        Type = TokenType.Long
                    });

                    inputOffset += longMatch.Length;
                }
                else
                {
                    token = token ?? new Token
                    {
                        Type = TokenType.Raw
                    };

                    token.Data.Add(source[inputOffset++]);

                    // Maximum block length
                    if (token.Data.Count == 69)
                    {
                        tokens.Add(token);
                        token = null;
                    }
                }
            }

            if (token != null)
                tokens.Add(token);

            return tokens;
        }

        /// <summary>
        /// Serialize the compression tokens.
        /// </summary>
        private static byte[] EncodeTokens(IEnumerable<Token> tokens)
        {
            var output = new List<byte>();
            var control = 0;
            var bits = 8;
            var buffer = new List<byte>();

            void AppendCommand(bool flag, IEnumerable<byte> bytes)
            {
                control >>= 1;
                if (flag)
                    control |= 0x80;

                buffer.AddRange(bytes);

                if (--bits != 0) 
                    return;

                bits = 8;
                output.Add(unchecked((byte) control));
                output.AddRange(buffer);
                buffer.Clear();
                control = 0;
            }

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Short:
                    {
                        var lengthComponent = (token.Length - 2) << 4;
                        var distanceComponent = token.Offset - 1;
                        AppendCommand(true, new[]
                        {
                            unchecked((byte) (0x80 | lengthComponent | distanceComponent))
                        });
                        break;
                    }
                    case TokenType.Long:
                    {
                        var lengthComponent = (token.Length - 3) << 10;
                        var distanceComponent = token.Offset;
                        var compositeComponent = lengthComponent | distanceComponent;
                        AppendCommand(true, new[]
                        {
                            unchecked((byte) (compositeComponent >> 8)),
                            unchecked((byte) compositeComponent)
                        });
                        break;
                    }
                    default:
                    {
                        if (token.Data.Count < 7)
                        {
                            foreach (var datum in token.Data)
                                AppendCommand(false, new[] {datum});
                        }
                        else
                        {
                            AppendCommand(true, new[] {(byte) (0xB9 + token.Data.Count)}.Concat(token.Data));
                        }

                        break;
                    }
                }
            }

            AppendCommand(true, new byte[] {0xFF});

            while (bits != 8)
                AppendCommand(true, Enumerable.Empty<byte>());

            return output.ToArray();            
        }

        public byte[] Encode(byte[] source)
        {
            var tokens = GetTokens(source);
            return EncodeTokens(tokens);
        }
    }
}