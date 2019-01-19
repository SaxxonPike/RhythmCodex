using System.Collections.Generic;
using System.IO;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Streamers
{
    public interface IXboxHbnStreamReader
    {
        IEnumerable<XboxHbnEntry> Read(Stream hbnStream, Stream binStream);
    }
}