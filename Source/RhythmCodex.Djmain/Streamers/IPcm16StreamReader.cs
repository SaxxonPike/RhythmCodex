using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IPcm16StreamReader
    {
        IList<byte> Read(Stream stream);
    }
}