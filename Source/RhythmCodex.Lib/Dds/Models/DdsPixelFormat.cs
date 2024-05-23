using RhythmCodex.Infrastructure;

namespace RhythmCodex.Dds.Models;

[Model]
public sealed class DdsPixelFormat
{
    public DdsPixelFormatFlags Flags { get; set; }
    public int FourCc { get; set; }
    public int BitCount { get; set; }
    public int RedMask { get; set; }
    public int GreenMask { get; set; }
    public int BlueMask { get; set; }
    public int AlphaMask { get; set; }
}