using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Heuristics;

[Service]
public class HeuristicTester(IEnumerable<IHeuristic> heuristics, ILogger logger) : IHeuristicTester
{
    public List<HeuristicResult> Match(Stream stream, long length, params Context[] contexts)
    {
        var cache = new CachedStream(stream);
        var result = new List<HeuristicResult>();
        foreach (var heuristic in GetHeuristics(contexts))
        {
            cache.Rewind();
            try
            {
                var match = heuristic.Match(new StreamHeuristicReader(cache));
                if (match != null)
                    result.Add(match);
            }
            catch (Exception e)
            {
                logger.Debug($"Exception in heuristic {heuristic.GetType().Name}{Environment.NewLine}{e}");
            }
        }

        return result;
    }

    public List<HeuristicResult> Match(Memory<byte> data, params Context[] contexts)
    {
        var result = new List<HeuristicResult>();
        foreach (var heuristic in GetHeuristics(contexts))
        {
            try
            {
                var match = heuristic.Match(new MemoryHeuristicReader(data));
                if (match != null)
                    result.Add(match);
            }
            catch (Exception e)
            {
                logger.Debug($"Exception in heuristic {heuristic.GetType().Name}{Environment.NewLine}{e}");
            }
        }

        return result;
    }

    private IEnumerable<IHeuristic> GetHeuristics(Context[] contexts)
    {
        return heuristics
            .Where(h => !contexts.Any() ||
                        h.GetType().GetCustomAttributes<ContextAttribute>()
                            .SelectMany(a => a.Contexts)
                            .Intersect(contexts).Any());
    }
}