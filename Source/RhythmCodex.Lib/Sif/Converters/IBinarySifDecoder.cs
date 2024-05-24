using System;
using RhythmCodex.Sif.Models;

namespace RhythmCodex.Sif.Converters;

public interface IBinarySifDecoder
{
    SifInfo Decode(ReadOnlyMemory<byte> bytes);
}