using System.IO;
using System.Linq;
using RhythmCodex.Dds.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Dds.Streamers
{
    [Service]
    public class DdsStreamReader : IDdsStreamReader
    {
        public const int MagicId = 0x20534444;
        
        public DdsImage Read(Stream source, int length)
        {
            var reader = new BinaryReader(source);

            var ddsId = reader.ReadInt32();
            if (ddsId != MagicId)
                throw new RhythmCodexException($"Invalid DDS identifier. Found: 0x{ddsId:X8}");

            var ddsSize = reader.ReadInt32();
            if (ddsSize != 124)
                throw new RhythmCodexException($"Invalid DDS size. Found: 0x{ddsSize:X8}");
            
            var result = new DdsImage
            {
                Flags = (DdsFlags) reader.ReadInt32(),
                Height = reader.ReadInt32(),
                Width = reader.ReadInt32(),
                PitchOrLinearSize = reader.ReadInt32(),
                Depth = reader.ReadInt32(),
                MipMapCount = reader.ReadInt32(),
                Reserved1 = Enumerable.Range(0, 11).Select(i => reader.ReadInt32()).ToArray(),
                PixelFormat = ReadPixelFormat(reader),
                Caps1 = (DdsCaps1) reader.ReadInt32(),
                Caps2 = reader.ReadInt32(),
                Caps3 = reader.ReadInt32(),
                Caps4 = reader.ReadInt32(),
                Reserved2 = reader.ReadInt32(),
                Data = reader.ReadBytes(length - 128)
            };

            return result;
        }

        private DdsPixelFormat ReadPixelFormat(BinaryReader reader)
        {
            var pfSize = reader.ReadInt32();
            if (pfSize != 32)
                throw new RhythmCodexException($"Invalid DDS pixel format size. Found: 0x{pfSize:X8}");
            
            return new DdsPixelFormat
            {
                Flags = (DdsPixelFormatFlags) reader.ReadInt32(),
                FourCc = reader.ReadInt32(),
                BitCount = reader.ReadInt32(),
                RedMask = reader.ReadInt32(),
                GreenMask = reader.ReadInt32(),
                BlueMask = reader.ReadInt32(),
                AlphaMask = reader.ReadInt32()
            };
        }
    }
}