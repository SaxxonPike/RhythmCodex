using System;

namespace RhythmCodex.Riff.Models;

public class RiffFormat : IRiffFormat
{
    public int Format { get; set; }
    public int Channels { get; set; }
    public int SampleRate { get; set; }
    public int ByteRate { get; set; }
    public int BlockAlign { get; set; }
    public int BitsPerSample { get; set; }
    public Memory<byte> ExtraData { get; set; }
}