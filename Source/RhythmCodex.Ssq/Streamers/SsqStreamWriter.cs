using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Streamers;

namespace RhythmCodex.Ssq.Streamers
{
    public class SsqStreamWriter : IStreamWriter<IEnumerable<Chunk?>>
    {
        private readonly IStreamWriter<Chunk?> _chunkStreamWriter;

        public SsqStreamWriter(IStreamWriter<Chunk?> chunkStreamWriter)
        {
            _chunkStreamWriter = chunkStreamWriter;
        }
        
        public void Write(Stream stream, IEnumerable<Chunk?> chunks)
        {
            foreach (var chunk in chunks)
                _chunkStreamWriter.Write(stream, chunk);

            _chunkStreamWriter.Write(stream, null);
        }
    }
}
