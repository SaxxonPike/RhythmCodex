using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

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

        public IEnumerable<HeuristicResult> Match(ReadOnlySpan<byte> data)
        {
            var result = new List<HeuristicResult>();
            foreach (var heuristic in _heuristics)
            {
                var match = heuristic.Match(data);
                if (match != null)
                    result.Add(match);
            }

            return result;
        }
    }
}