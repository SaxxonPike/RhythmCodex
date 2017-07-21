using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Streamers;

namespace RhythmCodex.Ssq.Streamers
{
    public class SsqStreamer : IStreamer<IList<IChunk>, IEnumerable<IChunk>>
    {
        private readonly IStreamer<IChunk, IChunk> _chunkStreamer;

        public SsqStreamer(IStreamer<IChunk, IChunk> chunkStreamer)
        {
            _chunkStreamer = chunkStreamer;
        }
        
        public IList<IChunk> Read(Stream stream)
        {
            var result = new List<IChunk>();
            
            while (true)
            {
                var chunk = _chunkStreamer.Read(stream);
                if (chunk == null)
                    return result;
                
                result.Add(chunk);
            }
        }

        public void Write(Stream stream, IEnumerable<IChunk> data)
        {
            foreach (var chunk in data)
                _chunkStreamer.Write(stream, chunk);
            
            _chunkStreamer.Write(stream, null);
        }
    }
}
