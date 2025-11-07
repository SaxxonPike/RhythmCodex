using System.IO;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

public interface IBeatmaniaPs2NewKeysoundStreamReader
{
    BeatmaniaPs2KeysoundSet Read(Stream stream);
}