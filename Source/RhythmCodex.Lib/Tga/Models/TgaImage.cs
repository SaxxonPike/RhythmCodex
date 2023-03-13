using RhythmCodex.Infrastructure;

namespace RhythmCodex.Tga.Models;
// Reference: http://paulbourke.net/dataformats/tga/

[Model]
public class TgaImage
{
    public int ColorMapType { get; set; }
    public TgaDataType DataTypeCode { get; set; }
    public int ColorMapOrigin { get; set; }
    public int ColorMapLength { get; set; }
    public byte ColorMapBitsPerEntry { get; set; }
    public int XOrigin { get; set; }
    public int YOrigin { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int BitsPerPixel { get; set; }
    public int AttributeBitsPerPixel { get; set; }
    public TgaInterleave Interleave { get; set; }
    public TgaOriginType OriginType { get; set; }
    public byte[] IdentificationField { get; set; }
    public byte[] ImageData { get; set; }
}