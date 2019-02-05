using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers
{
    public interface IBeatmaniaPs2NewKeysoundStreamReader
    {
        BeatmaniaPs2KeysoundSet Read(Stream stream);
    }
}