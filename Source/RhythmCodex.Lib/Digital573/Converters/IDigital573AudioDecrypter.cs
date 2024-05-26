using System;
using System.Collections.Generic;
using RhythmCodex.Digital573.Models;

namespace RhythmCodex.Digital573.Converters;

public interface IDigital573AudioDecrypter
{
    Digital573Audio DecryptNew(ReadOnlySpan<byte> input, IEnumerable<int> key);
    Digital573Audio DecryptOld(ReadOnlySpan<byte> input, int key1);
}