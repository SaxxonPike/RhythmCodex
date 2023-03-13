using RhythmCodex.Infrastructure;

namespace RhythmCodex.Dds.Models;

[Model]
public sealed class DdsImage
{
    public DdsFlags Flags { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int PitchOrLinearSize { get; set; }
    public int Depth { get; set; }
    public int MipMapCount { get; set; }
    public int[] Reserved1 { get; set; }
    public DdsPixelFormat PixelFormat { get; set; }
    public DdsCaps1 Caps1 { get; set; }
    public int Caps2 { get; set; }
    public int Caps3 { get; set; }
    public int Caps4 { get; set; }
    public int Reserved2 { get; set; }
    public byte[] Data { get; set; }
}