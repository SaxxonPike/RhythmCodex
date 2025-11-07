using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Graphics.Dds.Models;

[Model]
public sealed class DdsImage
{
    public DdsFlags Flags { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int PitchOrLinearSize { get; set; }
    public int Depth { get; set; }
    public int MipMapCount { get; set; }
    public Memory<int> Reserved1 { get; set; } = new int[11];
    public DdsPixelFormat PixelFormat { get; set; } = new();
    public DdsCaps1 Caps1 { get; set; }
    public int Caps2 { get; set; }
    public int Caps3 { get; set; }
    public int Caps4 { get; set; }
    public int Reserved2 { get; set; }
    public Memory<byte> Data { get; set; }
}