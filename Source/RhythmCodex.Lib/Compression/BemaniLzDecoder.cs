using System.IO;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Compression
{
    [Service]
    public class BemaniLzDecoder : IBemaniLzDecoder
    {
        private const int BufferMask = 0x3FF; // 10 bits window
        private const int BufferSize = 0x400;

        public void Decode(Stream source, Stream target)
        {
            using (var mem = new MemoryStream())
            {
                var writer = new BinaryWriter(mem);
                var reader = new BinaryReader(source);

                var buffer = new byte[BufferSize];
                var bufferOffset = 0;
                var control = 0; // used as flags
                var distance = 0; // used as a byte-distance
                var length = 0; // used as a counter

                while (true)
                {
                    var loop = false;

                    control >>= 1;
                    if (control < 0x100)
                        control = reader.ReadByte() | 0xFF00;

                    var data = reader.ReadByte();

                    // direct copy
                    if ((control & 1) == 0)
                    {
                        writer.Write(data);
                        buffer[bufferOffset] = data;
                        bufferOffset = (bufferOffset + 1) & BufferMask;
                        continue;
                    }

                    // long distance
                    if ((data & 0x80) == 0)
                    {
                        distance = reader.ReadByte() | ((data & 0x3) << 8);
                        length = (data >> 2) + 2;
                        loop = true;
                    }

                    // short distance
                    else if ((data & 0x40) == 0)
                    {
                        distance = (data & 0xF) + 1;
                        length = (data >> 4) - 7;
                        loop = true;
                    }

                    // loop for jumps
                    if (loop)
                    {
                        while (length-- >= 0)
                        {
                            data = buffer[(bufferOffset - distance) & BufferMask];
                            writer.Write(data);
                            buffer[bufferOffset] = data;
                            bufferOffset = (bufferOffset + 1) & BufferMask;
                        }

                        continue;
                    }

                    // end of stream
                    if (data == 0xFF)
                        break;

                    // block copy
                    length = data - 0xB9;
                    while (length >= 0)
                    {
                        data = reader.ReadByte();
                        writer.Write(data);
                        buffer[bufferOffset] = data;
                        bufferOffset = (bufferOffset + 1) & BufferMask;
                        length--;
                    }
                }

                writer = new BinaryWriter(target);
                writer.Write(mem.ToArray());
                writer.Flush();
            }

            target.Flush();
        }
    }
}