using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IAudioStreamReader
    {
        IList<byte> ReadDpcm(Stream stream);
        IList<byte> ReadPcm8(Stream stream);
        IList<byte> ReadPcm16(Stream stream);
    }
}