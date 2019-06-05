using System;

namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573DatabaseDecrypter
    {
        byte[] Decrypt(ReadOnlySpan<byte> database, string key);
    }
}