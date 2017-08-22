using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IPcm16StreamWriter
    {
        void Write(Stream stream, IEnumerable<byte> data);
    }
}