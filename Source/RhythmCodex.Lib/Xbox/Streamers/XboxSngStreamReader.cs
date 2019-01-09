using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Streamers
{
    [Service]
    public class XboxSngStreamReader : IXboxSngStreamReader
    {
        public IEnumerable<XboxSngEntry> Read(Stream stream)
        {
            var baseOffset = stream.Position;
            var reader = new BinaryReader(stream);
            var count = reader.ReadInt32();
            return Enumerable
                .Range(0, count)
                .Select(i =>
                {
                    stream.Position = baseOffset + 4 + (i * 0x14);

                    var result = new XboxSngEntry
                    {
                        Name = Encodings.UTF8.GetString(reader.ReadBytes(4))
                    };

                    var songOffset = reader.ReadInt32();
                    var songLength = reader.ReadInt32();
                    var previewOffset = reader.ReadInt32();
                    var previewLength = reader.ReadInt32();

                    stream.Position = baseOffset + songOffset;
                    result.Song = reader.ReadBytes(songLength);
                    if (previewLength > 0)
                    {
                        stream.Position = baseOffset + previewOffset;
                        result.Preview = reader.ReadBytes(previewLength);
                    }

                    return result;
                });
        }
    }
}