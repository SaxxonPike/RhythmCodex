using System;
using System.Collections.Generic;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.S573.Converters;

public interface IDdr573DatabaseDecoder
{
    List<DdrDatabaseEntry> Decode(ReadOnlySpan<byte> database);
    int FindRecordSize(ReadOnlySpan<byte> database);
}