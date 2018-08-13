using System.Collections.Generic;
using System.IO;
using RhythmCodex.Audio.Models;

namespace RhythmCodex.Audio.Streamers
{
    public interface IRiffStreamReader
    {
        IRiffContainer Read(Stream stream);
    }
}