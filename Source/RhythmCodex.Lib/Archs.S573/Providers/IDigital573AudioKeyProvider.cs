using System;
using RhythmCodex.Archs.S573.Models;

namespace RhythmCodex.Archs.S573.Providers;

public interface IDigital573AudioKeyProvider
{
    Digital573AudioKey? Get(ReadOnlySpan<byte> source);
}