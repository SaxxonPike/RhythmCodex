using System.Collections.Generic;
using RhythmCodex.Ddr.S573.Models;

namespace RhythmCodex.Ddr.S573.Converters;

public interface IDdr573ImageDecoder
{
    List<Ddr573File> Decode(Ddr573Image image, string? dbKey);
}