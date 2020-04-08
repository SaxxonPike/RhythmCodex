using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Heuristics
{
    [Service]
    public class HeuristicBlockStreamReader : IHeuristicBlockStreamReader
    {
        private readonly IHeuristicTester _heuristicTester;

        public HeuristicBlockStreamReader(IHeuristicTester heuristicTester)
        {
            _heuristicTester = heuristicTester;
        }

        public IEnumerable<HeuristicBlockResult> Find(Stream stream, long length, int blockSize,
            params Context[] contexts)
        {
            var cache = new CachedStream(stream);
            var offset = 0L;
            var max = length - blockSize;
            var block = new byte[blockSize];
            var index = 0;

            while (offset <= max)
            {
                if (stream.TryRead(block, 0, blockSize) < blockSize)
                    break;

                foreach (var result in _heuristicTester.Match(cache, length - offset, contexts))
                {
                    yield return new HeuristicBlockResult
                    {
                        BlockIndex = index,
                        Offset = offset,
                        Heuristic = result.Heuristic
                    };
                }
                offset += blockSize;
                cache.Advance(blockSize);
            }
        }
    }

    public interface IHeuristicBlockStreamReader
    {
        IEnumerable<HeuristicBlockResult> Find(Stream stream, long length, int blockSize, params Context[] contexts);
    }

    public class HeuristicBlockResult
    {
        public IHeuristic Heuristic { get; set; }
        public int BlockIndex { get; set; }
        public long Offset { get; set; }
    }
}