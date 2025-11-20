using System.Collections.Generic;
using RhythmCodex.Charts.Sm.Converters;
using RhythmCodex.Charts.Sm.Model;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Sif.Models;

namespace RhythmCodex.Metadatas.Sif.Converters;

[Service]
public class SifSmMetadataChanger(ISmMetadataChanger smMetadataChanger) : ISifSmMetadataChanger
{
    public void Apply(ICollection<Command> commands, SifInfo sif)
    {
        var dict = sif.KeyValues;

        if (dict.TryGetValue(SifKeys.Dir, out var name))
        {
            smMetadataChanger.SetBannerImage(commands, $"{name}_th.png");
            smMetadataChanger.SetBackgroundImage(commands, $"{name}_bk.png");
        }

        if (dict.TryGetValue(SifKeys.Title, out var value))
            smMetadataChanger.SetTitle(commands, value);
        if (dict.TryGetValue(SifKeys.Mix, out var value1))
            smMetadataChanger.SetSubtitle(commands, value1);
        if (dict.TryGetValue(SifKeys.Artist, out var value2))
            smMetadataChanger.SetArtist(commands, value2);
        if (dict.TryGetValue(SifKeys.Extra, out var value3))
            smMetadataChanger.SetSubartist(commands, value3);
        if (dict.ContainsKey(SifKeys.BpmMin) && dict.TryGetValue(SifKeys.BpmMax, out var value4))
            smMetadataChanger.SetBpm(commands, dict[SifKeys.BpmMin], value4);

        if (dict.ContainsKey(SifKeys.FootSingle))
        {
            var values = dict[SifKeys.FootSingle].Split(',');
            smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceSingle, SmNotesDifficulties.Easy,
                values[0]);
            smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceSingle, SmNotesDifficulties.Medium,
                values[1]);
            smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceSingle, SmNotesDifficulties.Hard,
                values[2]);
        }

        if (dict.ContainsKey(SifKeys.FootDouble))
        {
            var values = dict[SifKeys.FootDouble].Split(',');
            smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceDouble, SmNotesDifficulties.Easy,
                values[0]);
            smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceDouble, SmNotesDifficulties.Medium,
                values[1]);
            smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceDouble, SmNotesDifficulties.Hard,
                values[2]);
        }
    }
}