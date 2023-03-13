using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers;

public interface IBeatmaniaPcAudioEntryStreamReader
{
    BeatmaniaPcAudioEntry Read(Stream source);
}