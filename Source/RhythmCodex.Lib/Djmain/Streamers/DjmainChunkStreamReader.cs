using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RhythmCodex.Djmain.Heuristics;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Streamers
{
    [Service]
    public class DjmainChunkStreamReader : IDjmainChunkStreamReader
    {
        private readonly IDjmainHddDescriptionHeuristic _djmainHddDescriptionHeuristic;

        public DjmainChunkStreamReader(IDjmainHddDescriptionHeuristic djmainHddDescriptionHeuristic)
        {
            _djmainHddDescriptionHeuristic = djmainHddDescriptionHeuristic;
        }
        
        public IEnumerable<DjmainChunk> Read(Stream stream)
        {
            const int length = DjmainConstants.ChunkSize;
            var buffer = new byte[length];
            var id = 0;
            DjmainHddDescription format = null;

            while (true)
            {
                var offset = 0;
                var output = new byte[length];
                var outId = id++;

                while (offset < length)
                {
                    var bytesRead = stream.Read(buffer, offset, length - offset);
                    if (bytesRead == 0)
                        yield break;

                    offset += bytesRead;
                }

                buffer.AsSpan(0, length).CopyTo(output);

                if (format == null)
                    format = _djmainHddDescriptionHeuristic.Get(output);

                if (format.BytesAreSwapped)
                    output.AsSpan().Swap16();
                
                yield return new DjmainChunk
                {
                    Format = format.Format,
                    Data = output,
                    Id = outId
                };
            }
        }

    }
}