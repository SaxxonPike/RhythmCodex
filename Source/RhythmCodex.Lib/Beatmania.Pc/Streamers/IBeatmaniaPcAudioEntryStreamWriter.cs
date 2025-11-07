using System.IO;
using RhythmCodex.Beatmania.Pc.Models;

namespace RhythmCodex.Beatmania.Pc.Streamers;

public interface IBeatmaniaPcAudioEntryStreamWriter
{
    void Write(Stream target, BeatmaniaPcAudioEntry entry);
}