using System.IO;
using RhythmCodex.Audio.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Audio.Converters
{
    [Service]
    public class RiffFormatDecoder : IRiffFormatDecoder
    {
        public IRiffFormat Decode(IRiffChunk chunk)
        {
            using (var stream = new MemoryStream(chunk.Data))
            using (var reader = new BinaryReader(stream))
            {
                return new RiffFormat
                {
                    Format = reader.ReadInt16(),
                    Channels = reader.ReadInt16(),
                    SampleRate = reader.ReadInt32(),
                    ByteRate = reader.ReadInt32(),
                    BlockAlign = reader.ReadInt16(),
                    BitsPerSample = reader.ReadInt16(),
                    ExtraData = reader.ReadBytes(chunk.Data.Length - 16)
                };
            }
        }
    }
}