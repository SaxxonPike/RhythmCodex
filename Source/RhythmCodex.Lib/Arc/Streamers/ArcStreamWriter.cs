using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Arc.Model;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Arc.Streamers
{
    [Service]
    public class ArcStreamWriter : IArcStreamWriter
    {
        public void Write(Stream target, IEnumerable<ArcFile> files)
        {
            var fileList = files.AsList();
            using var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);

            writer.Write(0x19751120);
            writer.Write(0x00000001);
            writer.Write(fileList.Count);
            writer.Write(0x00000002);
            buffer.Position += fileList.Count * 0x10;

            var entries = fileList.Select(file =>
            {
                var entry = new ArcEntry
                {
                    CompressedSize = file.CompressedSize,
                    DecompressedSize = file.DecompressedSize,
                    NameOffset = (int) buffer.Position
                };
                writer.Write(Encodings.CP437.GetBytes(file.Name));
                writer.Write((byte) 0x00);
                return entry;
            }).ToList();

            void Pad0X20()
            {
                var padding = new byte[(0x20 - (buffer.Position & 0x1F)) & 0x1F];
                if (padding.Length > 0)
                    writer.Write(padding);
            }

            for (var i = 0; i < fileList.Count; i++)
            {
                Pad0X20();
                entries[i].Offset = (int) buffer.Position;
                writer.Write(fileList[i].Data);
            }

            Pad0X20();

            buffer.Position = 0x10;

            foreach (var entry in entries)
            {
                writer.Write(entry.NameOffset);
                writer.Write(entry.Offset);
                writer.Write(entry.DecompressedSize);
                writer.Write(entry.CompressedSize);
            }
            
            writer.Flush();
            target.Write(buffer.GetBuffer(), 0, (int) buffer.Length);
        }
    }
}