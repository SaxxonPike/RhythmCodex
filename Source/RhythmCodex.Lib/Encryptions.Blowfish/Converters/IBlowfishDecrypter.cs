using System;

namespace RhythmCodex.Encryptions.Blowfish.Converters;

public interface IBlowfishDecrypter
{
    Memory<byte> Decrypt(ReadOnlySpan<byte> data, string cipher);
}