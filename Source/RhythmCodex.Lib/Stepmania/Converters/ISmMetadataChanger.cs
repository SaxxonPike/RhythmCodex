using System.Collections.Generic;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Converters
{
    public interface ISmMetadataChanger
    {
        void SetTitle(ICollection<Command> commands, string value);
        void SetSubtitle(ICollection<Command> commands, string value);
        void SetArtist(ICollection<Command> commands, string value);
        void SetSubartist(ICollection<Command> commands, string value);
        void SetDifficulty(ICollection<Command> commands, string mode, string difficulty, string value);
        void SetBpm(ICollection<Command> commands, string minBpm, string maxBpm);
    }
}