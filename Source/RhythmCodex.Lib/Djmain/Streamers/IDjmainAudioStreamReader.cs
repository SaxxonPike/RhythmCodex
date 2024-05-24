using System;
using System.IO;

namespace RhythmCodex.Djmain.Streamers;

public interface IDjmainAudioStreamReader
{
    Memory<byte> ReadDpcm(Stream stream);
    Memory<byte> ReadPcm8(Stream stream);
    Memory<byte> ReadPcm16(Stream stream);
}