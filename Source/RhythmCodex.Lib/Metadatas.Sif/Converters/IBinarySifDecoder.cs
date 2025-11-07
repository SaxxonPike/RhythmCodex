using System;
using RhythmCodex.Metadatas.Sif.Models;

namespace RhythmCodex.Metadatas.Sif.Converters;

public interface IBinarySifDecoder
{
    SifInfo Decode(ReadOnlyMemory<byte> bytes);
}