using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IAudioStreamWriter
    {
        void WriteDpcm(Stream stream, IEnumerable<byte> data);
    }
}