using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression;

[Service]
public class BemaniLzEncoder : IBemaniLzEncoder
{
    private enum TokenType
    {
        Unassigned,
        Raw,
        Short,
        Long
    }

    private readonly struct Match
    {
        public int Offset { get; init; }
        public int Length { get; init; }
    }

    private struct Token
    {
        public int Offset { get; set; }
        public int Length { get; set; }
        public TokenType Type { get; set; }
        public IMemoryOwner<byte> Data { get; set; }
        public int DataLength { get; set; }
    }

    /// <summary>
    /// Read the source data and determine how to compress it.
    /// </summary>
    private static IEnumerable<Token> GetTokens(ReadOnlySpan<byte> source)
    {
        // Long:  0LLLLLDD DDDDDDDD (length 3-34, distance 0-1023)
        // Short: 10LLDDDD (length 2-5, distance 1-16)
        // Block: 11LLLLLL (length 8-70, 1:1)

        var inputLength = source.Length;
        var inputOffset = 0;

        var tokens = new List<Token>();
        var token = new Token();
        while (inputOffset < inputLength)
        {
            var shortMatch = FindMatch(source, inputOffset - 0x10, inputOffset, inputLength, 2, 4);
            var longMatch = FindMatch(source, inputOffset - 0x3FF, inputOffset, inputLength, 3, 33);

            var useShortMatch = shortMatch is { Offset: >= 0, Length: > 0 } &&
                                shortMatch.Length > longMatch.Length;
            var useLongMatch = longMatch is { Offset: >= 0, Length: > 0 } &&
                               longMatch.Length >= shortMatch.Length;

            if (useShortMatch)
            {
                if (token.Type != TokenType.Unassigned)
                    tokens.Add(token);
                token = new Token();

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
                if (token.Type != TokenType.Unassigned)
                    tokens.Add(token);
                token = new Token();

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
                if (token.Type == TokenType.Unassigned)
                {
                    token.Type = TokenType.Raw;
                    token.Data = MemoryPool<byte>.Shared.Rent(70);
                    token.DataLength = 0;
                }

                token.Data.Memory.Span[token.DataLength++] = source[inputOffset++];

                // Maximum block length
                if (token.DataLength >= 70)
                {
                    tokens.Add(token);
                    token = new Token();
                }
            }
        }

        if (token.Type != TokenType.Unassigned)
            tokens.Add(token);

        return tokens;

        Match FindMatch(ReadOnlySpan<byte> src, int lowIndex, int cursorIndex, int length,
            int matchMinLength, int matchMaxLength)
        {
            var bestOffset = -1;
            var bestLength = matchMinLength;
            int idx;
            int matchIdx;

            for (idx = Math.Max(0, lowIndex); idx < cursorIndex; idx++)
            {
                var match = true;
                var matchLength = 0;

                for (matchIdx = 0;
                     matchIdx < matchMaxLength && matchIdx + idx < length && cursorIndex + matchIdx < length;
                     matchIdx++)
                {
                    if (src[idx + matchIdx] != src[cursorIndex + matchIdx])
                    {
                        match = false;
                        break;
                    }

                    matchLength++;
                }

                if (!match || matchLength < bestLength) 
                    continue;

                bestLength = matchLength;
                bestOffset = idx;
                if (matchLength >= matchMaxLength)
                    break;
            }

            return new Match
            {
                Offset = bestOffset,
                Length = bestOffset < 0 ? 0 : bestLength
            };
        }
    }

    /// <summary>
    /// Serialize the compression tokens.
    /// </summary>
    private static byte[] EncodeTokens(IEnumerable<Token> tokens)
    {
        var output = new MemoryStream();
        var control = 0;
        var bits = 8;
        // Theoretical maximum of a command byte + all commands (70 * 8 + 1)
        var buffer = MemoryPool<byte>.Shared.Rent(561);
        var bufferLen = 0;

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Short:
                {
                    var lengthComponent = (token.Length - 2) << 4;
                    var distanceComponent = token.Offset - 1;
                    AppendBufferByte(unchecked((byte)(0x80 | lengthComponent | distanceComponent)));
                    CommitBuffer(true);
                    break;
                }
                case TokenType.Long:
                {
                    var lengthComponent = (token.Length - 3) << 10;
                    var distanceComponent = token.Offset;
                    var compositeComponent = lengthComponent | distanceComponent;
                    AppendBufferByte(unchecked((byte)(compositeComponent >> 8)));
                    AppendBufferByte(unchecked((byte)compositeComponent));
                    CommitBuffer(true);
                    break;
                }
                case TokenType.Raw:
                {
                    if (token.DataLength < 8)
                    {
                        var tokData = token.Data.Memory.Span;
                        for (var i = 0; i < token.DataLength; i++)
                        {
                            AppendBufferByte(tokData[i]);
                            CommitBuffer(false);
                        }
                    }
                    else
                    {
                        AppendBufferByte(unchecked((byte)(0xB8 + token.DataLength)));
                        AppendBufferBytes(token.Data.Memory.Span[..token.DataLength]);
                        CommitBuffer(true);
                    }

                    break;
                }
                default:
                {
                    throw new RhythmCodexException("Something went wrong with the tokenizer. Report this.");
                }
            }

            if (token.Data is { } tokHandle)
                tokHandle.Dispose();
        }

        AppendBufferByte(0xFF);
        CommitBuffer(true);

        while (bits != 8)
            CommitBuffer(true);

        var result = output.ToArray();
        return result;

        void AppendBufferByte(byte value)
        {
            buffer.Memory.Span[bufferLen] = value;
            bufferLen++;
        }

        void AppendBufferBytes(ReadOnlySpan<byte> values)
        {
            values.CopyTo(buffer.Memory.Span[bufferLen..]);
            bufferLen += values.Length;
        }

        void CommitBuffer(bool flag)
        {
            control >>= 1;
            if (flag)
                control |= 0x80;

            if (--bits != 0)
                return;

            bits = 8;
            output.WriteByte(unchecked((byte)control));
            output.Write(buffer.Memory.Span[..bufferLen]);
            bufferLen = 0;
            control = 0;
        }
    }

    public Memory<byte> Encode(ReadOnlySpan<byte> source)
    {
        var tokens = GetTokens(source);
        return EncodeTokens(tokens);
    }
}