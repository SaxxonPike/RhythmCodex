using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Step2.Models;

namespace RhythmCodex.Step2.Streamers
{
    [Service]
    public class Step2StreamReader : IStep2StreamReader
    {
        public Step2Chunk Read(Stream stream, int length)
        {
            var data = new BinaryReader(stream).ReadBytes(length);
            using (var mem = new ReadOnlyMemoryStream(data))
            using (var reader = new BinaryReader(mem))
            {
                var headerLength = reader.ReadInt32();
                if (headerLength < 4 || (headerLength & 3) != 0 || (headerLength - 4) % 0x14 != 0)
                    throw new RhythmCodexException("Incorrect Step2 header.");

                var metadataCount = (headerLength - 4) / 0x14;
                var metadatas = new List<Step2Metadata>();
                for (var i = 0; i < metadataCount; i++)
                {
                    metadatas.Add(new Step2Metadata
                    {
                        Offset = reader.ReadInt32() - headerLength,
                        Length = reader.ReadInt32() * 4,
                        Reserved = reader.ReadInt32(),
                        Next1P = (reader.ReadInt32() - 4) / 0x14,
                        Next2P = (reader.ReadInt32() - 4) / 0x14
                    });
                }

                return new Step2Chunk
                {
                    Metadatas = metadatas,
                    Data = reader.ReadBytes(length - headerLength)
                };
            }
        }
    }
}