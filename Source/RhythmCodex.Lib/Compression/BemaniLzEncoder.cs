using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression
{
    [Service]
    public class BemaniLzEncoder : IBemaniLzEncoder
    {
        public byte[] Encode(byte[] source)
        {
            // Long:  0LLLLLDD DDDDDDDD (length 3-34, distance 0-1023)
            // Short: 10LLDDDD (length 2-5, distance 1-16)
            // Block: 11LLLLLL (length 7-70, 1:1)

            (int Offset, int Length) FindMatch(int lowIndex, int highIndex, int cursorIndex, int length,
                int matchMinLength, int matchMaxLength)
            {
                var bestOffset = -1;
                var bestLength = matchMinLength;

                for (var idx = Math.Max(0, lowIndex); idx < highIndex; idx++)
                {
                    var match = true;
                    var matchLength = 0;

                    for (var matchIdx = 0; matchIdx < matchMaxLength && matchIdx + cursorIndex < length && matchIdx + idx < highIndex; matchIdx++)
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

                return (bestOffset, bestOffset < 0 ? 0 : bestLength);
            }

            var output = new List<byte>();
            var inputLength = source.Length;
            var inputOffset = 0;
            var control = 0;
            var bits = 8;
            var buffer = new List<byte>();
            var ended = false;

            while (true)
            {
                if (!ended && inputOffset >= inputLength)
                {
                    ended = true;
                    control |= 0x80;
                    buffer.Add(0xFF);
                }
                
                if (!ended)
                {
                    var shortMatch = FindMatch(inputOffset - 0x10, inputOffset, inputOffset, inputLength, 2, 4);
                    var longMatch = FindMatch(inputOffset - 0x3FF, inputOffset, inputOffset, inputLength, 3, 33);

                    var useShortMatch = shortMatch.Offset >= 0 &&
                                        shortMatch.Length > 0 &&
                                        shortMatch.Length > longMatch.Length;
                    var useLongMatch = longMatch.Offset >= 0 &&
                                       longMatch.Length > 0 &&
                                       longMatch.Length >= shortMatch.Length;

                    if (useShortMatch)
                    {
                        control |= 0x80;
                        var lengthComponent = (shortMatch.Length - 2) << 4;
                        var indicatorComponent = 0x80;
                        var distanceComponent = inputOffset - shortMatch.Offset - 1;
                        buffer.Add(unchecked((byte) (indicatorComponent | lengthComponent | distanceComponent)));
                        inputOffset += shortMatch.Length;
                    }
                    else if (useLongMatch)
                    {
                        control |= 0x80;
                        var lengthComponent = ((longMatch.Length - 3) << 10);
                        var distanceComponent = inputOffset - longMatch.Offset;
                        var compositeComponent = lengthComponent | distanceComponent;
                        buffer.Add(unchecked((byte) (compositeComponent >> 8)));
                        buffer.Add(unchecked((byte) compositeComponent));
                        inputOffset += longMatch.Length;
                    }
                    else
                    {
                        buffer.Add(source[inputOffset]);
                        inputOffset++;
                    }
                }

                bits--;
                if (bits == 0)
                {
                    bits = 8;
                    output.Add(unchecked((byte) control));
                    output.AddRange(buffer);
                    buffer.Clear();
                    control = 0;
                    if (ended)
                        break;
                }

                control >>= 1;
            }

            return output.ToArray();
        }
    }
}