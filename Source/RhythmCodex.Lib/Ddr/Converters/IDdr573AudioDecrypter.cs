using System;

namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573AudioDecrypter
    {
        byte[] DecryptNew(ReadOnlySpan<byte> input, params int[] key);
        byte[] DecryptOld(ReadOnlySpan<byte> input, int key1);
        string ExtractName(string sourceName);
    }
}