using System.Collections.Generic;
using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers
{
    public interface IBeatmaniaPcAudioStreamReader
    {
        IEnumerable<BeatmaniaPcAudioEntry> Read(Stream source, long length);
    }
}