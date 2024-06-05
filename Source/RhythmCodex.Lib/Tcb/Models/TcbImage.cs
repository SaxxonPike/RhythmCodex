using System;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Tcb.Models;

[Model]
public class TcbImage
{
    public int PaletteEntryCount { get; set; }
    public int ImageType { get; set; }
    public int MipmapCount { get; set; }
    public int PaletteType { get; set; }
    public int BitsPerPixel { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Memory<byte> GsTex { get; set; }
    public int GsRegs { get; set; }
    public int GsTexClut { get; set; }
    public Memory<byte> UserData { get; set; }
    public Memory<byte> Image { get; set; }
    public Memory<byte> Palette { get; set; }
}