using System.Collections.Generic;
using System.IO;
using RhythmCodex.Arc.Model;

namespace RhythmCodex.Arc.Streamers
{
    public interface IArcStreamReader
    {
        IEnumerable<ArcFile> Read(Stream source);
    }
}