using System.Collections.Generic;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Psx.Converters;

[Service]
public class PsxBeatmaniaSongGrouper : IPsxBeatmaniaSongGrouper
{
    public List<PsxBeatmaniaSongGroup> GroupFiles(IEnumerable<PsxBeatmaniaFile> files)
    {
        var groups = new List<PsxBeatmaniaSongGroup>();
        var currentGroup = new List<PsxBeatmaniaFile>();
        var groupIndex = 0;
        var prevGroupType = PsxBeatmaniaFileType.Unknown;

        foreach (var file in files)
        {
            if (prevGroupType == PsxBeatmaniaFileType.Keysound && file.Type != PsxBeatmaniaFileType.Keysound)
            {
                groups.Add(new PsxBeatmaniaSongGroup
                {
                    Index = groupIndex++,
                    Files = currentGroup
                });

                currentGroup = [];
            }

            prevGroupType = file.Type;
            currentGroup.Add(file);
        }

        if (currentGroup.Count > 0)
        {
            groups.Add(new PsxBeatmaniaSongGroup
            {
                Index = groupIndex,
                Files = currentGroup
            });
        }

        return groups;
    }
}

public interface IPsxBeatmaniaSongGrouper
{
    List<PsxBeatmaniaSongGroup> GroupFiles(IEnumerable<PsxBeatmaniaFile> files);
}