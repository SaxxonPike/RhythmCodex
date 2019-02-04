using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    [Service]
    public class SsqStreamReader : ISsqStreamReader
    {
        private readonly IChunkStreamReader _chunkStreamReader;

        public SsqStreamReader(IChunkStreamReader chunkStreamReader)
        {
            _chunkStreamReader = chunkStreamReader;
        }

        public IList<SsqChunk> Read(Stream stream)
        {
            return ReadInternal(stream).ToArray();
        }

        private IEnumerable<SsqChunk> ReadInternal(Stream stream)
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