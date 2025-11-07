using System.IO;
using RhythmCodex.Beatmania.Pc.Models;

namespace RhythmCodex.Beatmania.Pc.Streamers;

public interface IBeatmaniaPcAudioEntryStreamReader
{
    BeatmaniaPcAudioEntry Read(Stream source);
}