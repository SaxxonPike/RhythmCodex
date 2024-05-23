using System.Collections.Generic;
using System.IO;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Streamers;

public interface IXboxSngStreamReader
{
    IEnumerable<XboxSngEntry> Read(Stream stream);
}