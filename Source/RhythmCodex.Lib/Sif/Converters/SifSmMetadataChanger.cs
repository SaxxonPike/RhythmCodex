using System.Collections.Generic;
using RhythmCodex.Infrastructure;
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

            if (dict.ContainsKey("dir"))
            {
                var name = dict["dir"];
                _smMetadataChanger.SetBannerImage(commands, $"{name}_th.png");
                _smMetadataChanger.SetBackgroundImage(commands, $"{name}_bk.png");                
            }
                
            if (dict.ContainsKey("title"))
                _smMetadataChanger.SetTitle(commands, dict["title"]);
            if (dict.ContainsKey("mix"))
                _smMetadataChanger.SetSubtitle(commands, dict["mix"]);
            if (dict.ContainsKey("artist"))
                _smMetadataChanger.SetArtist(commands, dict["artist"]);
            if (dict.ContainsKey("extra"))
                _smMetadataChanger.SetSubartist(commands, dict["extra"]);
            if (dict.ContainsKey("bpm_min") && dict.ContainsKey("bpm_max"))
                _smMetadataChanger.SetBpm(commands, dict["bpm_min"], dict["bpm_max"]);

            if (dict.ContainsKey("foot.single"))
            {
                var values = dict["foot.single"].Split(',');
                _smMetadataChanger.SetDifficulty(commands, "dance-single", "easy", values[0]);
                _smMetadataChanger.SetDifficulty(commands, "dance-single", "medium", values[1]);
                _smMetadataChanger.SetDifficulty(commands, "dance-single", "hard", values[2]);
            }
                        
            if (dict.ContainsKey("foot.double"))
            {
                var values = dict["foot.double"].Split(',');
                _smMetadataChanger.SetDifficulty(commands, "dance-double", "easy", values[0]);
                _smMetadataChanger.SetDifficulty(commands, "dance-double", "medium", values[1]);
                _smMetadataChanger.SetDifficulty(commands, "dance-double", "hard", values[2]);
            }
        }
    }
}