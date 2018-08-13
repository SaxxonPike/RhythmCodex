using System.IO;
using System.Linq;
using RhythmCodex.Audio.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Audio.Streamers
{
    [Service]
    public class RiffStreamWriter : IRiffStreamWriter
    {
        public int Write(Stream stream, IRiffContainer container)
        {
            var writer = new BinaryWriter(stream, Encodings.CP437);
            
            // Validate.
            
            if (container.Format == null)
                throw new RhythmCodexException("Format string cannot be null.");

            var formatBytes = container.Format.GetBytes();
            
            if (formatBytes.Length != 4)
                throw new RhythmCodexException($"Format string must be 4 bytes. Found: '{container.Format}'");

            foreach (var chunk in container.Chunks)
            {
                if (chunk.Id == null)
                    throw new RhythmCodexException("Chunk ID string cannot be null.");

                var chunkIdBytes = chunk.Id.GetBytes();
                
                if (chunkIdBytes.Length != 4)
                    throw new RhythmCodexException($"Chunk ID string must be 4 bytes. Found: '{chunk.Id}'");
                
            }
            
            // Start writing out our data.

            var length = container.Chunks.Sum(c => c.Data.Length + 8);
            
            writer.Write("RIFF".GetBytes());
            writer.Write(length);
            writer.Write(container.Format.GetBytes());

            foreach (var chunk in container.Chunks)
            {
                writer.Write(chunk.Id.GetBytes());
                writer.Write(chunk.Data.Length);
                writer.Write(chunk.Data);
            }

            return length + 8;
        }
    }
}