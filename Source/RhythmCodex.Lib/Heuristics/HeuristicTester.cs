using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public IList<HeuristicResult> Match(ReadOnlySpan<byte> data, params Context[] contexts)
        {
            var result = new List<HeuristicResult>();
            foreach (var heuristic in _heuristics
                .Where(h => !contexts.Any() ||
                            h.GetType().GetCustomAttributes<ContextAttribute>()
                                .SelectMany(a => a.Contexts)
                                .Intersect(contexts).Any()
                )
            )
            {
                if (data.Length < heuristic.MinimumLength)
                    continue;

                var match = heuristic.Match(data);
                if (match != null)
                    result.Add(match);
            }

            return result;
        }
    }
}