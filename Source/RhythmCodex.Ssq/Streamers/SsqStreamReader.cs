using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
        public IList<IChunk> Read(Stream stream)
        {
            return ReadInternal(stream).ToArray();
        }

        private IEnumerable<IChunk> ReadInternal(Stream stream)
        {
            while (true)
            {
                var chunk = _chunkStreamReader.Read(stream);
                if (chunk == null)
                    yield break;
                
                yield return chunk;
            }            
        }
    }
}
