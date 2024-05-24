using System;

namespace RhythmCodex.Riff.Models;

public interface IRiffFormat
{
    int Format { get; }
    int Channels { get; }
    int SampleRate { get; }
    int ByteRate { get; }
    int BlockAlign { get; }
    int BitsPerSample { get; }
    Memory<byte> ExtraData { get; }
}