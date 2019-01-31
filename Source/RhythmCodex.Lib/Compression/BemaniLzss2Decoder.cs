using System;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Compression
{
    [Service]
    public class BemaniLzss2Decoder : IBemaniLzss2Decoder
    {
        private enum BemaniLzss2Type
        {
            GCZ,
            Firebeat
        }

        private struct BemaniLzss2Properties
        {
            public int RingBufferOffset;
            public int RingBufferSize;
            public BemaniLzss2Type Type;
        }

        private Memory<byte> Decompress(Stream source, int length, int decompLength, BemaniLzss2Properties props)
        {
            var ring = new byte[props.RingBufferSize];
            var ringPos = props.RingBufferOffset;
            var controlWord = 1;
            var controlBitsLeft = 0;
            const int controlBitMask = 0x1;

            if (decompLength <= 0)
                decompLength = int.MaxValue;

            var sourceReader = new BinaryReader(source);
            var target = new MemoryStream();
            var writer = new BinaryWriter(target);

            using (var mem = new MemoryStream(sourceReader.ReadBytes(length)))
            {
                var reader = new BinaryReader(mem);

                while (decompLength > 0 && length > 0)
                {
                    if (controlBitsLeft == 0)
                    {
                        /* Read a control byte */
                        controlWord = reader.ReadByte();
                        length--;
                        controlBitsLeft = 8;
                    }

                    /* Decode a byte according to the current control byte bit */
                    if ((controlWord & controlBitMask) != 0)
                    {
                        /* Straight copy, store into history ring */
                        var data = reader.ReadByte();
                        length--;

                        writer.Write(data);
                        ring[ringPos] = data;

                        ringPos = (ringPos + 1) % props.RingBufferSize;
                        decompLength--;
                    }
                    else
                    {
                        /* Reference to data in ring buffer */

                        byte cmd2;
                        byte cmd1;
                        int chunkLength;
                        int chunkOffset;
                        switch (props.Type)
                        {
                            case BemaniLzss2Type.Firebeat:
                                cmd1 = reader.ReadByte();
                                cmd2 = reader.ReadByte();
                                length -= 2;
                                chunkLength = (cmd1 & 0x0F) + 3;
                                chunkOffset = ((cmd1 & 0xF0) << 4) + cmd2;
                                chunkOffset = ringPos - chunkOffset;
                                while (chunkOffset < 0)
                                    chunkOffset += props.RingBufferSize;
                                break;
                            case BemaniLzss2Type.GCZ:
                                cmd1 = reader.ReadByte();
                                cmd2 = reader.ReadByte();
                                length -= 2;
                                chunkLength = (cmd2 & 0x0F) + 3;
                                chunkOffset = ((cmd2 & 0xF0) << 4) | cmd1;
                                break;
                            default:
                                return target.AsMemory();
                        }

                        for (; chunkLength > 0 && length > 0; chunkLength--)
                        {
                            /* Copy historical data to output AND current ring pos */
                            writer.Write(ring[chunkOffset]);
                            ring[ringPos] = ring[chunkOffset];

                            /* Update counters */
                            chunkOffset = (chunkOffset + 1) % props.RingBufferSize;
                            ringPos = (ringPos + 1) % props.RingBufferSize;
                            decompLength--;
                        }
                    }

                    /* Get next control bit */
                    controlWord >>= 1;
                    controlBitsLeft--;
                }
            }

            return target.AsMemory();
        }

        public Memory<byte> DecompressFirebeat(Stream source, int length, int decompLength)
        {
            var props = new BemaniLzss2Properties
            {
                RingBufferOffset = 0xFFE, 
                RingBufferSize = 0x1000, 
                Type = BemaniLzss2Type.Firebeat
            };
            return Decompress(source, length, decompLength, props);
        }

        public Memory<byte> DecompressGcz(Stream source, int length, int decompLength)
        {
            var props = new BemaniLzss2Properties
            {
                RingBufferOffset = 0xFFE, 
                RingBufferSize = 0x1000, 
                Type = BemaniLzss2Type.GCZ
            };
            return Decompress(source, length, decompLength, props);
        }
    }
}