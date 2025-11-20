using System.Collections.Generic;
using RhythmCodex.Charts.Sm.Model;

namespace RhythmCodex.Charts.Sm.Converters;

public interface ISmMetadataChanger
{
    void SetTitle(ICollection<Command> commands, string value);
    void SetSubtitle(ICollection<Command> commands, string value);
    void SetArtist(ICollection<Command> commands, string value);
    void SetSubartist(ICollection<Command> commands, string value);
    void SetDifficulty(ICollection<Command> commands, string mode, string difficulty, string value);
    void SetBpm(ICollection<Command> commands, string minBpm, string maxBpm);
    void SetBannerImage(ICollection<Command> commands, string fileName);
    void SetBackgroundImage(ICollection<Command> commands, string fileName);
}