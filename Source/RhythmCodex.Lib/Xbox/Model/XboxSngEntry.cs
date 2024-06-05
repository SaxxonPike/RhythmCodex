using System;

namespace RhythmCodex.Xbox.Model;

public class XboxSngEntry
{
    public string? Name { get; set; }
    public Memory<byte> Preview { get; set; }
    public Memory<byte> Song { get; set; }
}