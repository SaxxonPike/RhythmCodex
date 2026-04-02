using System.IO;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

public interface IBeatmaniaPs2NewChartStreamReader
{
    BeatmaniaPs2Chart Read(Stream stream, long length);
}