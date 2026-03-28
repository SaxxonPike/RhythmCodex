using System.Collections.Generic;
using RhythmCodex.Games.Beatmania.Psx.Models;

namespace RhythmCodex.Games.Beatmania.Psx.Converters;

public interface IPsxBeatmaniaSongGrouper
{
    List<PsxBeatmaniaSongGroup> GroupFiles(IEnumerable<PsxBeatmaniaFile> files);
}