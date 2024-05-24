using System;

namespace RhythmCodex.Blowfish.Converters;

public interface IBlowfishDecrypter
{
    Memory<byte> Decrypt(ReadOnlySpan<byte> data, string cipher);
}