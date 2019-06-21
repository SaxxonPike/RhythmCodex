using System;
using System.Collections.Generic;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573DatabaseDecoder
    {
        IList<Ddr573DatabaseEntry> Decode(ReadOnlySpan<byte> database);
        int FindRecordSize(ReadOnlySpan<byte> database);
    }
}