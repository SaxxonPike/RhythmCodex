using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Arc.Model;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Arc.Streamers
{
    // source: arcunpack (gergc)
    
    [Service]
    public class ArcStreamReader : IArcStreamReader
    {
        public IEnumerable<ArcFile> Read(Stream source)
        {
            if (!source.CanSeek)
                throw new RhythmCodexException("Stream must be seekable (for now)");

            return ReadInternal(source);
        }

        private static IEnumerable<ArcFile> ReadInternal(Stream source)
        {
            var baseOffset = source.Position;
            var reader = new BinaryReader(source);
            var header = ReadHeader(reader);
            
            if (header.Id != 0x19751120)
                throw new RhythmCodexException("Unrecognized arc ID");
            
            var directory = Enumerable.Range(0, header.FileCount).Select(_ => ReadEntry(reader)).ToList();

            foreach (var entry in directory)
            {
                // TODO: consider if it's worth doing a forward-only implementation
                source.Position = baseOffset + entry.NameOffset;
                var nameBytes = source.ReadZeroTerminated();
                var name = nameBytes.GetString();
                source.Position = baseOffset + entry.Offset;
                yield return new ArcFile
                {
                    Data = reader.ReadBytes(entry.CompressedSize),
                    CompressedSize = entry.CompressedSize,
                    DecompressedSize = entry.DecompressedSize,
                    Name = name
                };
            }
        }

        private static ArcHeader ReadHeader(BinaryReader reader)
        {
            return new ArcHeader
            {
                Id = reader.ReadInt32(),
                Unk0 = reader.ReadInt32(),
                FileCount = reader.ReadInt32(),
                Unk1 = reader.ReadInt32()
            };
        }

        private static ArcEntry ReadEntry(BinaryReader reader)
        {
            return new ArcEntry
            {
                NameOffset = reader.ReadInt32(),
                Offset = reader.ReadInt32(),
                DecompressedSize = reader.ReadInt32(),
                CompressedSize = reader.ReadInt32()
            };
        }
    }
}