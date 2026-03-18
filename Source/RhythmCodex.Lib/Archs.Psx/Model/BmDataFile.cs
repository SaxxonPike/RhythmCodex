using System;
using System.Collections.Generic;

namespace RhythmCodex.Archs.Psx.Model;

public record BmDataFile
{
    public int Index { get; set; }
    public int Group { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
    public BmDataPakEntryType Type { get; set; }
}