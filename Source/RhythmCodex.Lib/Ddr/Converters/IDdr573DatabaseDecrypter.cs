using System;

namespace RhythmCodex.Ddr.Converters;

public interface IDdr573DatabaseDecrypter
{
    int ConvertKey(string key);
    int FindKey(ReadOnlySpan<byte> database);
    byte[] Decrypt(ReadOnlySpan<byte> database, int key);
}