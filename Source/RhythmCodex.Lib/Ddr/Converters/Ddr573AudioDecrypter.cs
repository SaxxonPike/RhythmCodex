using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class Ddr573AudioDecrypter : IDdr573AudioDecrypter
    {
        private static int Bit(int value, int n) =>
            (value >> n) & 1;

        private static readonly Dictionary<char, char> NameMap = new Dictionary<char, char>
        {
            {'A', '0'},
            {'B', 'A'},
            {'C', '1'},
            {'D', 'B'},
            {'E', '2'},
            {'F', 'C'},
            {'G', '3'},
            {'H', 'D'},
            {'I', '4'},
            {'J', 'E'},
            {'K', '5'},
            {'L', 'F'},
            {'M', '6'},
            {'N', 'G'},
            {'O', '7'},
            {'P', 'H'},
            {'Q', '8'},
            {'R', 'I'},
            {'S', '9'},
            {'T', 'J'},
            {'0', 'K'},
            {'1', 'L'},
            {'2', 'M'},
            {'3', 'N'},
            {'4', 'O'},
            {'5', 'P'},
            {'6', 'Q'},
            {'7', 'R'},
            {'8', 'S'},
            {'9', 'T'},
            {'U', 'U'},
            {'V', 'V'},
            {'W', 'W'},
            {'X', 'X'},
            {'Y', 'Y'},
            {'Z', 'Z'},
            {'_', '_'}
        };

        private static int BitSwap16(int v, int b15, int b14, int b13, int b12, int b11, int b10, int b9, int b8,
            int b7, int b6, int b5, int b4, int b3, int b2, int b1, int b0) =>
            (Bit(v, b15) << 15) |
            (Bit(v, b14) << 14) |
            (Bit(v, b13) << 13) |
            (Bit(v, b12) << 12) |
            (Bit(v, b11) << 11) |
            (Bit(v, b10) << 10) |
            (Bit(v, b9) << 9) |
            (Bit(v, b8) << 8) |
            (Bit(v, b7) << 7) |
            (Bit(v, b6) << 6) |
            (Bit(v, b5) << 5) |
            (Bit(v, b4) << 4) |
            (Bit(v, b3) << 3) |
            (Bit(v, b2) << 2) |
            (Bit(v, b1) << 1) |
            (Bit(v, b0) << 0);

        public byte[] DecryptNew(ReadOnlySpan<byte> input, params int[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (key.Length < 3)
                throw new RhythmCodexException($"Key must have at least 3 values. Found: {key.Length}");

            var length = input.Length & ~1;
            var output = new byte[length];

            var key1 = key[0];
            var key2 = key[1];
            var key3 = key[2];
            for (var i = 0; i < length; i += 2, key3++)
            {
                var v = input[i] | (input[i + 1] << 8);
                var m = key1 ^ key2;

                v = BitSwap16(
                    v,
                    15 - Bit(m, 0xF),
                    14 + Bit(m, 0xF),
                    13 - Bit(m, 0xE),
                    12 + Bit(m, 0xE),
                    11 - Bit(m, 0xB),
                    10 + Bit(m, 0xB),
                    9 - Bit(m, 0x9),
                    8 + Bit(m, 0x9),
                    7 - Bit(m, 0x8),
                    6 + Bit(m, 0x8),
                    5 - Bit(m, 0x5),
                    4 + Bit(m, 0x5),
                    3 - Bit(m, 0x3),
                    2 + Bit(m, 0x3),
                    1 - Bit(m, 0x2),
                    0 + Bit(m, 0x2)
                );

                v = (
                        v ^
                        (Bit(m, 0xD) << 14) ^
                        (Bit(m, 0xC) << 12) ^
                        (Bit(m, 0xA) << 10) ^
                        (Bit(m, 0x7) << 8) ^
                        (Bit(m, 0x6) << 6) ^
                        (Bit(m, 0x4) << 4) ^
                        (Bit(m, 0x1) << 2) ^
                        (Bit(m, 0x0) << 0)
                    ) & 0xFFFF;

                v = v ^ BitSwap16(
                        key3,
                        7, 0, 6, 1,
                        5, 2, 4, 3,
                        3, 4, 2, 5,
                        1, 6, 0, 7
                    );

                output[i] = unchecked((byte) (v >> 8));
                output[i + 1] = unchecked((byte) v);

                key1 = (
                           (key1 & 0x8000) |
                           ((key1 << 1) & 0x7FFE) |
                           ((key1 >> 14) & 1)
                       ) & 0xFFFF;

                if ((((key1 >> 15) ^ key1) & 1) != 0)
                {
                    key2 = (
                               (key2 << 1) |
                               (key2 >> 15)
                           ) & 0xFFFF;
                }
            }

            return output;
        }

        public byte[] DecryptOld(ReadOnlySpan<byte> data, int keyValue)
        {
            var seed = BitSwap16(keyValue,
                0xD, 0xB, 0x9, 0x7,
                0x5, 0x3, 0x1, 0xF,
                0xE, 0xC, 0xA, 0x8,
                0x6, 0x4, 0x2, 0x0);
            var key = new byte[0x10];
            key[0] = unchecked((byte) seed);
            key[1] = unchecked((byte) (seed >> 8));
            for (var i = 2; i < 16; i++)
            {
                var j = key[i - 2];
                key[i] = unchecked((byte) ((j << 1) | (j >> 7)));
            }

            var counter = 0;
            var dataLen = data.Length & ~1;
            var output = new byte[dataLen];
            var outputIdx = 0;
            var keyIdx = 0;
            var curKey = key[0xF];

            for (var idx = 0; idx < dataLen; idx += 2)
            {
                var outputWord = 0;
                var curData = (data[idx + 1] << 8) | data[idx];
                var curScramble = curKey;
                curKey = key[keyIdx & 0xF];
                keyIdx++;

                for (var curBit = 0; curBit < 8; curBit++)
                {
                    var evenBitShift = (curBit << 1) & 0xFF;
                    var oddBitShift = ((curBit << 1) + 1) & 0xFF;
                    var isEvenBitSet = (curData & (1 << evenBitShift)) != 0;
                    var isOddBitSet = (curData & (1 << oddBitShift)) != 0;
                    var isKeyBitSet = (curKey & (1 << curBit)) != 0;
                    var isScrambleBitSet = (curScramble & (1 << curBit)) != 0;

                    if (isScrambleBitSet)
                    {
                        var temp = isEvenBitSet;
                        isEvenBitSet = isOddBitSet;
                        isOddBitSet = temp;
                    }

                    if (isEvenBitSet ^ isKeyBitSet)
                        outputWord |= 1 << evenBitShift;
                    if (isOddBitSet)
                        outputWord |= 1 << oddBitShift;
                }

                output[outputIdx] = unchecked((byte) (outputWord >> 8));
                output[outputIdx + 1] = unchecked((byte) outputWord);
                outputIdx += 2;
                counter = (counter + 1) & 0xFF;
            }

            return output;
        }

        public string ExtractName(string sourceName)
        {
            if (sourceName == null)
                throw new ArgumentNullException(nameof(sourceName));
            var prunedSourceName = Path.GetFileNameWithoutExtension(sourceName.ToUpperInvariant());
            if (prunedSourceName.Length != 8)
                return prunedSourceName;
            var output = string.Empty;

            switch (prunedSourceName[0])
            {
                case 'E':
                case 'M':
                case 'S':
                {
                    var nameToDecode = $"{prunedSourceName[6]}{prunedSourceName[5]}" +
                                       $"{prunedSourceName[4]}{prunedSourceName[7]}_{prunedSourceName[3]}";
                    if (nameToDecode.Any(c => !NameMap.ContainsKey(c)))
                        return null;
                    foreach (var c in nameToDecode)
                    {
                        var dc = NameMap[c];
                        if (output.Length < 4 || (dc >= '0' && dc <= '9'))
                            output += dc;
                    }
                    break;
                }

                default:
                    return null;
            }

            switch (prunedSourceName[0])
            {
                case 'E':
                    output = $"system-{output}";
                    break;
                case 'S':
                    output += "-preview";
                    break;
            }

            return output;
        }
    }
}