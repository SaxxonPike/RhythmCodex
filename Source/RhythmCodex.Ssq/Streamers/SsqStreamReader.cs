using System.Collections.Generic;
using System.IO;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Streamers;

namespace RhythmCodex.Ssq.Streamers
{
    public class SsqStreamReader : IStreamReader<IEnumerable<Chunk?>>
    {
        private readonly IStreamReader<Chunk?> _chunkStreamReader;

        public SsqStreamReader(IStreamReader<Chunk?> chunkStreamReader)
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
