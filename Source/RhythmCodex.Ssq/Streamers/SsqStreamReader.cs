using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    public class SsqStreamReader : ISsqStreamReader
    {
        private readonly IChunkStreamReader _chunkStreamReader;

        public SsqStreamReader(IChunkStreamReader chunkStreamReader)
        {
            _chunkStreamReader = chunkStreamReader;
        }
        
        public IEnumerable<Chunk?> Read(Stream stream)
        {
            var result = new List<Chunk?>();
            
            while (true)
            {
                var chunk = _chunkStreamReader.Read(stream);
                if (chunk == null)
                    return result;
                
                result.Add(chunk);
            }
        }
    }
}
