using System;
using RhythmCodex.Digital573.Models;

namespace RhythmCodex.Digital573.Providers;

public interface IDigital573AudioKeyProvider
{
    Digital573AudioKey? Get(ReadOnlySpan<byte> source);
}