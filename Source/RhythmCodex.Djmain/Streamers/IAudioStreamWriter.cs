using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers
{
    public interface IAudioStreamWriter
    {
        void WriteDpcm(Stream stream, IEnumerable<byte> data);
        void WritePcm8(Stream stream, IEnumerable<byte> data);
        void WritePcm16(Stream stream, IEnumerable<byte> data);
    }
}