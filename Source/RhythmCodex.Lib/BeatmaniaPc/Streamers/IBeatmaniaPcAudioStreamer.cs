using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.BeatmaniaPc.Streamers
{
    public interface IBeatmaniaPcAudioStreamer
    {
        byte[] Decrypt(Stream source, long length);
        IEnumerable<ISound> Read(Stream source, long length);
    }
}