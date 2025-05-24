using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using RhythmCodex.Digital573.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Digital573.Converters;

// Revised algorithm source:
// https://github.com/mamedev/mame/blob/master/src/mame/konami/k573fpga.cpp

[Service]
public class Digital573AudioDecrypter : IDigital573AudioDecrypter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Bit(int value, int n) =>
        (value >> n) & 1;

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

    private static int DecryptCommon(int data, int key) =>
        BitSwap16(
            data,
            15 - Bit(key, 0xF),
            14 + Bit(key, 0xF),
            13 - Bit(key, 0xD),
            12 + Bit(key, 0xD),
            11 - Bit(key, 0xB),
            10 + Bit(key, 0xB),
            9 - Bit(key, 0x9),
            8 + Bit(key, 0x9),
            7 - Bit(key, 0x7),
            6 + Bit(key, 0x7),
            5 - Bit(key, 0x5),
            4 + Bit(key, 0x5),
            3 - Bit(key, 0x3),
            2 + Bit(key, 0x3),
            1 - Bit(key, 0x1),
            0 + Bit(key, 0x1)
        ) ^ (key & 0x5555);

    public Digital573Audio DecryptNew(ReadOnlySpan<byte> input, params int[] key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (key.Length < 3)
            throw new RhythmCodexException($"Key must have at least 3 values. Found: {key.Length}");

        var length = input.Length & ~1;
        var output = new byte[length];

        var keyBytes = new byte[4];
        BinaryPrimitives.WriteInt16LittleEndian(keyBytes, unchecked((short)key[0]));
        BinaryPrimitives.WriteInt16LittleEndian(keyBytes.AsSpan(2), unchecked((short)key[1]));

        var key1 = key[0];
        var key2 = key[1];
        var key3 = key[2];

        for (var i = 0; i < length; i += 2, key3++)
        {
            var m = BitSwap16(
                key1 ^ key2,
                15, 13, 14, 12,
                11, 10, 9, 7,
                8, 6, 5, 4,
                3, 1, 2, 0
            );

            var v = DecryptCommon(BinaryPrimitives.ReadUInt16LittleEndian(input[i..]), m);

            v ^= BitSwap16(
                key3,
                7, 0, 6, 1,
                5, 2, 4, 3,
                3, 4, 2, 5,
                1, 6, 0, 7
            );

            BinaryPrimitives.WriteUInt16BigEndian(output.AsSpan(i), unchecked((ushort)v));

            if ((Bit(key1, 14) ^ Bit(key1, 15)) != 0)
                key2 = ((key2 << 1) | (key2 >> 15)) & 0xFFFF;

            key1 = (
                (key1 & 0x8000) |
                ((key1 << 1) & 0x7FFE) |
                ((key1 >> 14) & 1)
            ) & 0xFFFF;
        }

        return new Digital573Audio
        {
            Key = keyBytes,
            Counter = key[2],
            Data = output
        };
    }

    public Digital573Audio DecryptOld(ReadOnlySpan<byte> data, int key)
    {
        var length = data.Length & ~1;
        var output = new byte[length];

        var keyBytes = new byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(keyBytes, key);

        var key1 = key;

        for (var i = 0; i < length; i += 2)
        {
            var v = DecryptCommon(BinaryPrimitives.ReadUInt16LittleEndian(data[i..]), key1);
            BinaryPrimitives.WriteUInt16BigEndian(output.AsSpan(i), unchecked((ushort)v));
            key1 = ((key1 << 1) | (key1 >> 15)) & 0xFFFF;
        }

        return new Digital573Audio
        {
            Counter = 0,
            Data = output,
            Key = keyBytes
        };
    }

}