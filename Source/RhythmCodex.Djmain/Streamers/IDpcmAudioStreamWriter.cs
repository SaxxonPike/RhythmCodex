using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IDpcmAudioStreamWriter
    {
        void Write(Stream stream, IEnumerable<byte> data);
    }
}