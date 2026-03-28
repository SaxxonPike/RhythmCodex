using System.Collections.Generic;
using RhythmCodex.Games.Beatmania.Psx.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Beatmania.Psx.Converters;

[Service]
public class PsxBeatmaniaSongGrouper : IPsxBeatmaniaSongGrouper
{
    public List<PsxBeatmaniaSongGroup> GroupFiles(IEnumerable<PsxBeatmaniaFile> files)
    {
        var groups = new List<PsxBeatmaniaSongGroup>();
        var currentGroup = new List<PsxBeatmaniaFile>();
        var groupNumber = 0;
        var groupIndex = 0;
        var endOfGroup = false;

        foreach (var file in files)
        {
            if (endOfGroup)
            {
                if (file.Type is not (PsxBeatmaniaFileType.Keysound or PsxBeatmaniaFileType.Kst))
                    Commit();
            }
            else
            {
                switch (file.Type)
                {
                    case PsxBeatmaniaFileType.Graphics:
                        Commit();
                        break;
                    case PsxBeatmaniaFileType.Keysound or PsxBeatmaniaFileType.Kst:
                        endOfGroup = true;
                        break;
                }
            }

            currentGroup.Add(file with
            {
                Group = groupNumber,
                GroupIndex = groupIndex++
            });
        }

        Commit();

        return groups;

        void Commit()
        {
            if (currentGroup.Count <= 0)
                return;

            groups.Add(new PsxBeatmaniaSongGroup
            {
                Index = groupNumber++,
                Files = currentGroup
            });

            currentGroup = [];
            endOfGroup = false;
            groupIndex = 0;
        }
    }
}