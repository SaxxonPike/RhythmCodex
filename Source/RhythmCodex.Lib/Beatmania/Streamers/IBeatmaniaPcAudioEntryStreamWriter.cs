using System.IO;
using RhythmCodex.Beatmania.Models;

namespace RhythmCodex.Beatmania.Streamers
{
    public interface IBeatmaniaPcAudioEntryStreamWriter
    {
        void Write(Stream target, BeatmaniaPcAudioEntry entry);
    }
}