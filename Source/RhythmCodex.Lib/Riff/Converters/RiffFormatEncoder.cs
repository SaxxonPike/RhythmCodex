using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Converters
{
    [Service]
    public class RiffFormatEncoder : IRiffFormatEncoder
    {
        public IRiffChunk Encode(IRiffFormat format)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(unchecked((short)format.Format));
                writer.Write(unchecked((short)format.Channels));
                writer.Write(format.SampleRate);
                writer.Write(format.ByteRate);
                writer.Write(unchecked((short)format.BlockAlign));
                writer.Write(unchecked((short)format.BitsPerSample));
                writer.Write(format.ExtraData);
                writer.Flush();
                
                return new RiffChunk
                {
                    Id = "fmt ",
                    Data = stream.ToArray()
                };
            }
        }
    }
}