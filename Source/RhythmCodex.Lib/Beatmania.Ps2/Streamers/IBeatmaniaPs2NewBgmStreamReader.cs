using System.IO;
using RhythmCodex.Beatmania.Ps2.Models;

namespace RhythmCodex.Beatmania.Ps2.Streamers;

public interface IBeatmaniaPs2NewBgmStreamReader
{
    BeatmaniaPs2Bgm Read(Stream stream);
}