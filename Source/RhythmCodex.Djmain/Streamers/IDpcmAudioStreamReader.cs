using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IDpcmAudioStreamReader
    {
        IList<byte> Read(Stream stream);
    }
}