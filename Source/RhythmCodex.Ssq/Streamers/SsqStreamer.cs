using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Streamers;

namespace RhythmCodex.Ssq.Streamers
{
    public class SsqStreamer : IStreamer<IEnumerable<Chunk?>>
    {
        private readonly IStreamer<Chunk?> _chunkStreamer;

        public SsqStreamer(IStreamer<Chunk?> chunkStreamer)
        {
            _chunkStreamer = chunkStreamer;
        }
        
        public IEnumerable<Chunk?> Read(Stream stream)
        {
            var result = new List<Chunk?>();
            
            while (true)
            {
                var chunk = _chunkStreamer.Read(stream);
                if (chunk == null)
                    return result;
                
                result.Add(chunk);
            }
        }

        public void Write(Stream stream, IEnumerable<Chunk?> data)
        {
            foreach (var chunk in data)
                _chunkStreamer.Write(stream, chunk);
            
            _chunkStreamer.Write(stream, null);
        }
    }
}
