using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Tga.Models;

namespace RhythmCodex.Tga.Streamers
{
    [Service]
    public class TgaStreamReader : ITgaStreamReader
    {
        public TgaImage Read(Stream source, long length)
        {
            var reader = new BinaryReader(source);

            var idFieldLength = reader.ReadByte();
            var colorMapType = reader.ReadByte();
            var imageTypeCode = reader.ReadByte();
            var colorMapOrigin = reader.ReadInt16();
            var colorMapLength = reader.ReadInt16();
            var colorMapEntrySize = reader.ReadByte();
            var xOrigin = reader.ReadInt16();
            var yOrigin = reader.ReadInt16();
            var width = reader.ReadInt16();
            var height = reader.ReadInt16();
            var pixelSize = reader.ReadByte();
            var descriptor = reader.ReadByte();
            var idField = reader.ReadBytes(idFieldLength);
            var data = reader.ReadBytes((int)(length - idFieldLength - 0x12));
            
            return new TgaImage
            {
                ColorMapType = colorMapType,
                DataTypeCode = (TgaDataType) imageTypeCode,
                ColorMapOrigin = colorMapOrigin,
                ColorMapLength = colorMapLength,
                ColorMapBitsPerEntry = colorMapEntrySize,
                XOrigin = xOrigin,
                YOrigin = yOrigin,
                Width = width,
                Height = height,
                BitsPerPixel = pixelSize,
                AttributeBitsPerPixel = descriptor & 0xF,
                OriginType = (TgaOriginType) ((descriptor & 0x20) >> 5),
                Interleave = (TgaInterleave) ((descriptor & 0xC0) >> 6),
                IdentificationField = idField,
                ImageData = data
            };
        }
    }
}