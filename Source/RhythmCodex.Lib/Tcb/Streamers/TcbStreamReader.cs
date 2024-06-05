using System.IO;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Tcb.Models;

namespace RhythmCodex.Tcb.Streamers;
// Source: https://github.com/root670/ddr-tools/blob/master/tcb-convert.c

[Service]
public class TcbStreamReader : ITcbStreamReader
{
    public TcbImage? Read(Stream stream)
    {
        var reader = new BinaryReader(stream);

        var id = reader.ReadInt32();
        if (id != 0x424354)
            return null;

        var padding = reader.ReadBytes(12);
        if (padding.Any(x => x != 0x00))
            return null;

        var totalLength = reader.ReadInt32();
        var paletteLength = reader.ReadInt32();
        var imageDataLength = reader.ReadInt32();
        int headerLength = reader.ReadUInt16();
        int numPaletteEntries = reader.ReadUInt16();

        int imageType = reader.ReadByte();
        int numMipmaps = reader.ReadByte();
        int paletteType = reader.ReadByte();
        int bitsPerPixel = reader.ReadByte();
        int width = reader.ReadUInt16();
        int height = reader.ReadUInt16();
        var tex = reader.ReadBytes(16);
        var regs = reader.ReadInt32();
        var texClut = reader.ReadInt32();
        var userData = reader.ReadBytes(headerLength - 0x20);
        var bitmap = reader.ReadBytes(imageDataLength);
        var palette = reader.ReadBytes(paletteLength);

        var extraBytes = totalLength - 0x10 - paletteLength - imageDataLength - headerLength;
        if (extraBytes > 0)
            reader.ReadBytes(extraBytes);

        return new TcbImage
        {
            PaletteEntryCount = numPaletteEntries,
            ImageType = imageType,
            MipmapCount = numMipmaps,
            PaletteType = paletteType,
            BitsPerPixel = bitsPerPixel,
            Width = width,
            Height = height,
            GsTex = tex,
            GsRegs = regs,
            GsTexClut = texClut,
            Image = bitmap,
            Palette = palette,
            UserData = userData
        };
    }
}