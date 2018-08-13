using System.Collections.Generic;
using System.IO;
using RhythmCodex.Audio.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Audio.Streamers
{
    public class RiffChunkStreamReader : IRiffChunkStreamReader
    {
        public IRiffChunk Read(Stream stream)
        {
            var reader = new BinaryReader(stream, Encodings.CP437);
            var type = new string(reader.ReadChars(4));
            var length = reader.ReadInt32();
            var data = reader.ReadBytes(length);
            
            return new RiffChunk
            {
                Id = type,
                Data = data
            };
        }
    }
}