using System;
using RhythmCodex.Archs.S573.Models;

namespace RhythmCodex.Archs.S573.Converters;

public interface IDigital573AudioDecrypter
{
    Digital573Audio DecryptNew(ReadOnlySpan<byte> input, Digital573AudioKey key);
    Digital573Audio DecryptOld(ReadOnlySpan<byte> input, int key1);
}