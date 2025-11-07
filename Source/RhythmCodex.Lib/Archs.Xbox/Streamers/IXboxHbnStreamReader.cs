using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Xbox.Model;

namespace RhythmCodex.Archs.Xbox.Streamers;

public interface IXboxHbnStreamReader
{
    IEnumerable<XboxHbnEntry> Read(Stream hbnStream, Stream binStream);
}