using System;
using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Djmain.Streamers;

public interface IDjmainAudioStreamWriter
{
    void WriteDpcm(Stream stream, ReadOnlySpan<byte> data);
    void WritePcm8(Stream stream, ReadOnlySpan<byte> data);
    void WritePcm16(Stream stream, ReadOnlySpan<byte> data);
}