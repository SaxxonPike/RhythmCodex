using System.Collections.Generic;
using System.IO;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Streamers
{
    public interface IXboxKasStreamReader
    {
        IEnumerable<XboxKasEntry> Read(Stream kasStream);
    }
}