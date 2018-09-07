using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.BeatmaniaPsx.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.BeatmaniaPsx.Streamers
{
    [Service]
    public class BeatmaniaPsxBmDataStreamReader : IBeatmaniaPsxBmDataStreamReader
    {
        private readonly ILogger _logger;

        public BeatmaniaPsxBmDataStreamReader(ILogger logger)
        {
            _logger = logger;
        }
        
        public IList<BeatmaniaPsxFolder> Read(Stream stream, int length)
        {
            return ReadInternal(stream, length).ToList();
        }

        private IEnumerable<BeatmaniaPsxFolder> ReadInternal(Stream stream, int length)
        {
            _logger.Debug($"{nameof(BeatmaniaPsxBmDataStreamReader)}: reading stream, 0x{length:X8} bytes");
            
            var reader = new BinaryReader(stream);
            var offset = 0;
            while (offset < length - 0x7FF)
            {
                var directorySize = reader.ReadInt32();
                if (directorySize < 1)
                {
                    reader.ReadBytes(0x7FC);
                    offset += 0x800;
                    continue;
                }

                var directoryEntryCount = reader.ReadInt32();
                if (directoryEntryCount < 1)
                {
                    reader.ReadBytes(0x7F8);
                    offset += 0x800;
                    continue;
                }
                
                _logger.Debug($"{nameof(BeatmaniaPsxBmDataStreamReader)}: found directory, offset=0x{offset:X8} fileCount={directoryEntryCount} directorySize=0x{(directorySize * 0x800):X4}");

                var directoryEntries = Enumerable
                    .Range(0, directoryEntryCount)
                    .Select(i => new BeatmaniaPsxDirectoryEntry
                    {
                        Offset = reader.ReadInt32() - directorySize,
                        Length = reader.ReadInt32()
                    })
                    .ToList();

                offset += directorySize * 0x800;
                var directoryPadding = directorySize * 0x800 - directoryEntryCount * 8 - 8;
                reader.ReadBytes(directoryPadding);

                var maxSize = directoryEntries.Max(de => (de.Offset * 0x800) + de.Length);
                if ((maxSize & 0x7FF) != 0)
                    maxSize = (maxSize & ~0x7FF) + 0x800;

                var buffer = reader.ReadBytes(maxSize);
                offset += maxSize;

                var files = new List<BeatmaniaPsxFile>();
                using (var mem = new MemoryStream(buffer))
                using (var memReader = new BinaryReader(mem))
                {
                    foreach (var entry in directoryEntries)
                    {
                        mem.Position = entry.Offset * 0x800;
                        files.Add(new BeatmaniaPsxFile
                        {
                            Data = memReader.ReadBytes(entry.Length)
                        });
                    }
                }

                yield return new BeatmaniaPsxFolder
                {
                    Files = files
                };
            }
        }
    }
}