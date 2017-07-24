using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public interface ISsqStreamWriter
    {
        void Write(Stream stream, IEnumerable<Chunk?> chunks);
    }
}