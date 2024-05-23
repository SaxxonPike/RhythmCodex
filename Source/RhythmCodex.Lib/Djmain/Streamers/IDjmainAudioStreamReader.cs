using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers;

public interface IDjmainAudioStreamReader
{
    byte[] ReadDpcm(Stream stream);
    byte[] ReadPcm8(Stream stream);
    byte[] ReadPcm16(Stream stream);
}