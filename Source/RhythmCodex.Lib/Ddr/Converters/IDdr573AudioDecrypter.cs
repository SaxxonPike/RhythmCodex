using System;

namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573AudioDecrypter
    {
        byte[] DecryptNew(ReadOnlySpan<byte> input, int key1, int key2, int key3);
        byte[] DecryptOld(ReadOnlySpan<byte> input, int key1);
    }
}