using System;
using System.Collections.Generic;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Converters;

public interface IDdr573DatabaseDecoder
{
    List<DdrDatabaseEntry> Decode(ReadOnlySpan<byte> database);
    int FindRecordSize(ReadOnlySpan<byte> database);
}