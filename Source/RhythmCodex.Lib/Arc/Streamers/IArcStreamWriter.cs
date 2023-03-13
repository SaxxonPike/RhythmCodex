using System.Collections.Generic;
using System.IO;
using RhythmCodex.Arc.Model;

namespace RhythmCodex.Arc.Streamers;

public interface IArcStreamWriter
{
    void Write(Stream target, IEnumerable<ArcFile> files);
}