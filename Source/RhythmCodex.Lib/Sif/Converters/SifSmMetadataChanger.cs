using System.Collections.Generic;
using RhythmCodex.IoC;
using RhythmCodex.Sif.Models;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Sif.Converters
{
    [Service]
    public class SifSmMetadataChanger : ISifSmMetadataChanger
    {
        private readonly ISmMetadataChanger _smMetadataChanger;

        public SifSmMetadataChanger(ISmMetadataChanger smMetadataChanger)
        {
            _smMetadataChanger = smMetadataChanger;
        }

        public void Apply(ICollection<Command> commands, SifInfo sif)
        {
            var dict = sif.KeyValues;

            if (dict.ContainsKey(SifKeys.Dir))
            {
                var name = dict[SifKeys.Dir];
                _smMetadataChanger.SetBannerImage(commands, $"{name}_th.png");
                _smMetadataChanger.SetBackgroundImage(commands, $"{name}_bk.png");
            }

            if (dict.ContainsKey(SifKeys.Title))
                _smMetadataChanger.SetTitle(commands, dict[SifKeys.Title]);
            if (dict.ContainsKey(SifKeys.Mix))
                _smMetadataChanger.SetSubtitle(commands, dict[SifKeys.Mix]);
            if (dict.ContainsKey(SifKeys.Artist))
                _smMetadataChanger.SetArtist(commands, dict[SifKeys.Artist]);
            if (dict.ContainsKey(SifKeys.Extra))
                _smMetadataChanger.SetSubartist(commands, dict[SifKeys.Extra]);
            if (dict.ContainsKey(SifKeys.BpmMin) && dict.ContainsKey(SifKeys.BpmMax))
                _smMetadataChanger.SetBpm(commands, dict[SifKeys.BpmMin], dict[SifKeys.BpmMax]);

            if (dict.ContainsKey(SifKeys.FootSingle))
            {
                var values = dict[SifKeys.FootSingle].Split(',');
                _smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceSingle, SmNotesDifficulties.Easy,
                    values[0]);
                _smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceSingle, SmNotesDifficulties.Medium,
                    values[1]);
                _smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceSingle, SmNotesDifficulties.Hard,
                    values[2]);
            }

            if (dict.ContainsKey(SifKeys.FootDouble))
            {
                var values = dict[SifKeys.FootDouble].Split(',');
                _smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceDouble, SmNotesDifficulties.Easy,
                    values[0]);
                _smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceDouble, SmNotesDifficulties.Medium,
                    values[1]);
                _smMetadataChanger.SetDifficulty(commands, SmGameTypes.DanceDouble, SmNotesDifficulties.Hard,
                    values[2]);
            }
        }
    }
}