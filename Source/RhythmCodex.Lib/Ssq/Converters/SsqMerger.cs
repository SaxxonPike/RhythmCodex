using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class SsqMerger : ISsqMerger
    {
        public IList<SsqChunk> Merge(IEnumerable<SsqChunk> target, IEnumerable<SsqChunk> items)
        {
            var result = target.GroupBy(c => c.Parameter0)
                .ToDictionary(c => c.Key, c => (IEnumerable<SsqChunk>) c);
            var source = items.GroupBy(c => c.Parameter0)
                .ToDictionary(c => c.Key, c => (IEnumerable<SsqChunk>) c);

            foreach (var (key, value) in source)
                result[key] = value;

            return result.SelectMany(c => c.Value).ToList();
        }
    }
}