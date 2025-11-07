using System;

namespace RhythmCodex.Sounds.Riff.Models;

public class RiffFormat
{
    public int Format { get; set; }
    public int Channels { get; set; }
    public int SampleRate { get; set; }
    public int ByteRate { get; set; }
    public int BlockAlign { get; set; }
    public int BitsPerSample { get; set; }
    public Memory<byte> ExtraData { get; set; }
}