using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Tim.Converters
{
    [Service]
    public class TimDataDecoder : ITimDataDecoder
    {
        public int[] Decode4Bit(byte[] data, int stride, int height)
        {
            using (var stream = new ReadOnlyMemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var size = height * stride * 2;
                var result = new int[size];
                for (var i = 0; i < size; i++)
                {
                    var pixels = reader.ReadByte();
                    result[i++] = pixels & 0xF;
                    result[i] = pixels >> 4;
                }

                return result;                
            }
        }

        public int[] Decode8Bit(byte[] data, int stride, int height)
        {
            using (var stream = new ReadOnlyMemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var size = height * stride;
                return reader.ReadBytes(size).Select(b => (int) b).ToArray();                
            }
        }

        public int[] Decode16Bit(byte[] data, int stride, int height)
        {
            using (var stream = new ReadOnlyMemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var size = height * stride / 2;
                var result = new int[size];
                for (var i = 0; i < size; i++)
                    result[i] = reader.ReadUInt16();
                return result;                
            }
        }

        public int[] Decode24Bit(byte[] data, int stride, int height)
        {
            using (var stream = new ReadOnlyMemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var size = height * stride / 3;
                var result = new int[size];
                for (var i = 0; i < size; i++)
                {
                    var pixels = reader.ReadBytes(3);
                    result[i] = pixels[0] | (pixels[1] << 8) | (pixels[2] << 16);
                }
                return result;                
            }
        }
    }
}