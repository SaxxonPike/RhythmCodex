using System.Collections.Generic;
using RhythmCodex.Ddr.Models;

namespace RhythmCodex.Ddr.Converters;

public interface IDdr573ImageDirectoryDecoder
{
    List<Ddr573DirectoryEntry> Decode(Ddr573Image image);
}