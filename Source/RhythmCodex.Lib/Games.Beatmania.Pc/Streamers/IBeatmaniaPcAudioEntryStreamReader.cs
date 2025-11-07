using System.IO;
using RhythmCodex.Games.Beatmania.Pc.Models;

namespace RhythmCodex.Games.Beatmania.Pc.Streamers;

public interface IBeatmaniaPcAudioEntryStreamReader
{
    BeatmaniaPcAudioEntry Read(Stream source);
}