using System;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression
{
    [Service]
    public class BemaniLzssDecoder : IBemaniLzssDecoder
    {
        private const int BufferMask = 0x3FFF; // 14 bits window
        private const int BufferSize = 0x4000;

        public Memory<byte> Decode(Stream source)
        {
            using (var mem = new MemoryStream())
            {
                var writer = new BinaryWriter(mem);
                var reader = new BinaryReader(source);
                var buffer = new byte[BufferSize];
                var bufferOffset = 0;

                var control = 0;
                var offset = 0;

                while (true)
                {
                    control >>= 1;
                    if (control < 0x100)
                        control = reader.ReadByte() | 0xFF00;

                    var data = reader.ReadByte();
                    var length = 0;
                    if ((control & 0x1) != 0)
                    {
                        if ((data & 0x80) != 0)
                        {
                            // 1DDDDDDD
                            writer.Write(data);
                            buffer[bufferOffset] = data;
                            bufferOffset = (bufferOffset + 1) & BufferMask;
                            continue;
                        }
                        else if ((data & 0x40) != 0)
                        {
                            // 01FFFFFF FFFFFFFF
                            offset = reader.ReadByte() | ((data & 0x3F) << 8);
                            length = 4;
                        }
                        else
                        {
                            // 00FFFFFF FFFFFFFF LLLLLLLL
                            offset = reader.ReadByte() | ((data & 0x3F) << 8);
                            length = 5 + reader.ReadByte();
                        }
                    }
                    else
                    {
                        if ((data & 0x80) != 0)
                        {
                            if ((data & 0x40) != 0)
                            {
                                if (data == 0xFF)
                                {
                                    // 11111111
                                    break;
                                }
                                else
                                {
                                    // 11FFFLLL FFFFFFFF
                                    offset = ((data & 0x38) << 5) | reader.ReadByte();
                                    length = (data & 0x07) + 5;
                                }
                            }
                            else
                            {
                                // 10FFFFFF FFFFFFFF
                                offset = reader.ReadByte() | ((data & 0x3F) << 8);
                                length = 3;
                            }
                        }
                        else
                        {
                            // 0DDDDDDD
                            writer.Write(data);
                            buffer[bufferOffset] = data;
                            bufferOffset = (bufferOffset + 1) & BufferMask;
                            continue;
                        }
                    }

                    offset++;
                    while (length > 0)
                    {
                        data = reader.ReadByte();
                        writer.Write(data);
                        buffer[bufferOffset] = data;
                        bufferOffset = (bufferOffset + 1) & BufferMask;
                        length--;
                    }
                }

                writer.Flush();
                return mem.AsMemory();
            }
        }
    }
}