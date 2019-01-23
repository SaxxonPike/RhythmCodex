using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Heuristics
{
    [Service]
    public class HeuristicTester : IHeuristicTester
    {
        private readonly IEnumerable<IHeuristic> _heuristics;

        public HeuristicTester(IEnumerable<IHeuristic> heuristics)
        {
            _heuristics = heuristics;
        }

        public IEnumerable<IHeuristic> Find(ReadOnlySpan<byte> data)
        {
            var result = new List<IHeuristic>();
            foreach (var heuristic in _heuristics)
            {
                if (heuristic.IsMatch(data))
                    result.Add(heuristic);
            }

            return result;
        }
    }
}