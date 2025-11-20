using System;

namespace RhythmCodex.Games.Ddr.S573.Converters;

public interface IDdr573DatabaseDecrypter
{
    int ConvertKey(string key);
    int FindKey(ReadOnlySpan<byte> database);
    Memory<byte> Decrypt(ReadOnlySpan<byte> database, int key);
}