using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Converters
{
    [Service]
    public class RiffFormatDecoder : IRiffFormatDecoder
    {
        public IRiffFormat Decode(IRiffChunk chunk)
        {
            using (var stream = new ReadOnlyMemoryStream(chunk.Data))
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