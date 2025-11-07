using System.Collections.Generic;
using RhythmCodex.Ddr.S573.Models;

namespace RhythmCodex.Ddr.S573.Converters;

public interface IDdr573ImageDirectoryDecoder
{
    List<Ddr573DirectoryEntry> Decode(Ddr573Image image);
}