using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IPcm8AudioStreamReader
    {
        IList<byte> Read(Stream stream);
    }
}