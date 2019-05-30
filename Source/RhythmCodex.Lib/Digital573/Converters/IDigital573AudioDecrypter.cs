using System;

namespace RhythmCodex.Digital573.Converters
{
    public interface IDigital573AudioDecrypter
    {
        byte[] DecryptNew(ReadOnlySpan<byte> input, params int[] key);
        byte[] DecryptOld(ReadOnlySpan<byte> input, int key1);
    }
}