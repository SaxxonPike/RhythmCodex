using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IDjmainAudioStreamReader
    {
        IList<byte> ReadDpcm(Stream stream);
        IList<byte> ReadPcm8(Stream stream);
        IList<byte> ReadPcm16(Stream stream);
    }
}