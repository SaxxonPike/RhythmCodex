using System;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Streamers;

[Service]
public class EncryptedBeatmaniaPcAudioStreamReader : IEncryptedBeatmaniaPcAudioStreamReader
{
    // reference: crack2dx.c (thanks Tau)
    // borrowed from Scharfrichter

    private enum BeatmaniaPcAudioEncryptionType
    {
        Standard,
        Partial
    }

    private static readonly byte[] EncryptionKey9 =
    [
        0x97, 0x1E, 0x24, 0xA0, 0x9A, 0x00, 0x10, 0x2B,
        0x91, 0xEF, 0xD7, 0x7A, 0xCD, 0x11, 0xAF, 0xAF,
        0x8D, 0x26, 0x5D, 0xBB, 0xE0, 0xC6, 0x1B, 0x2B
    ];

    private static readonly byte[] EncryptionKey10 =
    [
        0x2D, 0x86, 0x56, 0x62, 0xD7, 0xFD, 0xCA, 0xA4,
        0xB3, 0x24, 0x60, 0x26, 0x24, 0x81, 0xDB, 0xC2,
        0x57, 0xB1, 0x74, 0x6F, 0xA7, 0x52, 0x99, 0x21
    ];

    private static readonly byte[] EncryptionKey11 =
    [
        0xED, 0xF0, 0x9C, 0x90, 0x44, 0x1A, 0x5A, 0x03,
        0xAB, 0x07, 0xC1, 0x99, 0x23, 0x24, 0x32, 0xC7,
        0x5F, 0x32, 0xA5, 0x97, 0xAD, 0x98, 0x0F, 0x8F
    ];

    private static readonly byte[] EncryptionKey16 =
    [
        0x28, 0x22, 0x28, 0x54, 0x63, 0x3F, 0x0E, 0x42,
        0x6F, 0x45, 0x4E, 0x50, 0x67, 0x53, 0x61, 0x7C,
        0x04, 0x46, 0x00, 0x3B, 0x13, 0x2B, 0x45, 0x6A
    ];

    public Memory<byte> Decrypt(Stream source, long length)
    {
        if (length < 4)
            throw new RhythmCodexException($"{nameof(length)} must be at least 4.");

        var reader = new BinaryReader(source);
        byte[] key;
        BeatmaniaPcAudioEncryptionType encType;
        var headerChars = reader.ReadChars(4);
        var headerId = new string(headerChars);

        switch (headerId)
        {
            case @"%eNc":
                key = EncryptionKey9;
                encType = BeatmaniaPcAudioEncryptionType.Standard;
                break;
            case @"%e10":
                key = EncryptionKey10;
                encType = BeatmaniaPcAudioEncryptionType.Standard;
                break;
            case @"%e11":
                key = EncryptionKey11;
                encType = BeatmaniaPcAudioEncryptionType.Standard;
                break;
            case @"%e12":
                key = EncryptionKey11;
                encType = BeatmaniaPcAudioEncryptionType.Partial;
                break;
            case @"%hid":
                key = EncryptionKey11;
                encType = BeatmaniaPcAudioEncryptionType.Partial;
                break;
            case @"%iO0":
                key = EncryptionKey16;
                encType = BeatmaniaPcAudioEncryptionType.Standard;
                break;
            default:
                return source.TryRead(4, (int)(length - 4));
        }

        {
            var filelength = reader.ReadInt32();
            var fileExtraBytes = (8 - filelength % 8) % 8;
            var data = reader.ReadBytes(filelength + fileExtraBytes);
            reader.ReadBytes((int)(length - data.Length - 8));
            using var encodedDataMem = new ReadOnlyMemoryStream(data);
            return DecryptInternal(encodedDataMem, key, encType, data.Length);
        }
    }

    private static byte[] DecryptInternal(Stream source, ReadOnlySpan<byte> key,
        BeatmaniaPcAudioEncryptionType type, int length)
    {
        var output = new byte[length];
        var outputIndex = 0;
        Span<byte> block = stackalloc byte[8];
        Span<byte> lastBlock = stackalloc byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        Span<byte> currentBlock = stackalloc byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

        while (source.Position < source.Length)
        {
            source.ReadAtLeast(block, 8);
            block.CopyTo(currentBlock);

            // xor with key 0
            for (var i = 0; i < 8; i++)
                block[i] ^= key[i];

            // manipulation
            DecryptCycle(block);

            // swap first half with second half
            for (var i = 0; i < 4; i++)
            {
                (block[i], block[i + 4]) = (block[i + 4], block[i]);
            }

            if (type == BeatmaniaPcAudioEncryptionType.Standard)
            {
                // xor with key 1
                for (var i = 0; i < 8; i++)
                    block[i] ^= key[8 + i];

                // manipulation
                DecryptCycle(block);

                // xor with key 2
                for (var i = 0; i < 8; i++)
                    block[i] ^= key[16 + i];
            }

            // xor with previous state
            for (var i = 0; i < 8; i++)
                block[i] ^= lastBlock[i];

            // output
            block.CopyTo(output.AsSpan(outputIndex));
            outputIndex += 8;
            currentBlock.CopyTo(lastBlock);
        }

        return output;
    }

    private static void DecryptCycle(Span<byte> block)
    {
        unchecked
        {
            var a = (block[0] * 63) & 0xFF;
            var b = (a + block[3]) & 0xFF;
            var c = (block[1] * 17) & 0xFF;
            var d = (c + block[2]) & 0xFF;
            var e = (d + b) & 0xFF;
            var f = (e * block[3]) & 0xFF;
            var g = (f + b + 51) & 0xFF;
            var h = (b ^ d) & 0xFF;
            var i = (g ^ e) & 0xFF;

            block[4] ^= (byte)h;
            block[5] ^= (byte)d;
            block[6] ^= (byte)i;
            block[7] ^= (byte)g;
        }
    }
}