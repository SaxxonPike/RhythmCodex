using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers;

public interface IBeatmaniaPs2NewBgmStreamReader
{
    BeatmaniaPs2Bgm Read(Stream stream);
}