using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxBeatmaniaSongGrouper
{
    List<PsxBeatmaniaSongGroup> GroupFiles(IEnumerable<PsxBeatmaniaFile> files);
}