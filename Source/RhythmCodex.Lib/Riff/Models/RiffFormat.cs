using System;

namespace RhythmCodex.Riff.Models;

public record RiffFormat
{
    public int Format { get; init; }
    public int Channels { get; init; }
    public int SampleRate { get; init; }
    public int ByteRate { get; init; }
    public int BlockAlign { get; init; }
    public int BitsPerSample { get; init; }
    public ReadOnlyMemory<byte> ExtraData { get; init; }
}