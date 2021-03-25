using System.Collections.Generic;
using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXsbSoundStreamReader
    {
        XsbSound Read(Stream stream);
    }
}