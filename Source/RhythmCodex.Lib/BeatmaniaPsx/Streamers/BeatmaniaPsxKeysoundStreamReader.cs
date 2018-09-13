using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.BeatmaniaPsx.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.BeatmaniaPsx.Streamers
{
    public class BeatmaniaPsxKeysoundStreamReader : IBeatmaniaPsxKeysoundStreamReader
    {
        private readonly IVagStreamReader _vagStreamReader;

        public BeatmaniaPsxKeysoundStreamReader(IVagStreamReader vagStreamReader)
        {
            _vagStreamReader = vagStreamReader;
        }
        
        public IList<BeatmaniaPsxKeysound> Read(Stream stream)
        {
            var reader = new BinaryReaderEx(stream);

            reader.ReadInt32S(); // directory offset
            var directoryLength = reader.ReadInt32S();
            reader.ReadInt32S();
            reader.ReadInt32S();

            var keysounds = Enumerable
                .Range(0, directoryLength / 0x10)
                .Select(i => new BeatmaniaPsxKeysound
                {
                    DirectoryEntry = new BeatmaniaPsxKeysoundDirectoryEntry
                    {
                        Offset = reader.ReadInt32(),
                        Unknown0 = reader.ReadInt32(),
                        Unknown1 = reader.ReadInt32(),
                        Unknown2 = reader.ReadInt32()
                    }
                })
                .ToList();
            
            var dataOffset = reader.ReadInt32S();
            var dataLength = reader.ReadInt32S();
            reader.ReadInt32S();
            reader.ReadInt32S();

            var data = reader.ReadBytes(dataLength);
            using (var dataStream = new MemoryStream(data))
            {
                foreach (var keysound in keysounds)
                {
                    dataStream.Position = keysound.DirectoryEntry.Offset - dataOffset;
                    keysound.Data = _vagStreamReader.Read(dataStream, 1, 16);
                }
            }

            return keysounds;
        }
    }
}