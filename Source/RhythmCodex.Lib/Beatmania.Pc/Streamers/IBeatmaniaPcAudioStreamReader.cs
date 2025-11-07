using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Pc.Models;

namespace RhythmCodex.Beatmania.Pc.Streamers;

public interface IBeatmaniaPcAudioStreamReader
{
    IEnumerable<BeatmaniaPcAudioEntry> Read(Stream source, long length);
}