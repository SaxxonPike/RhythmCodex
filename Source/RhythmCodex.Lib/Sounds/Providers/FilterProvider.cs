using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;

namespace RhythmCodex.Sounds.Providers;

[Service]
public class FilterProvider(IList<IFilter> filters) : IFilterProvider
{
    public IEnumerable<IFilter> Get(FilterType type) => 
        filters.OrderByDescending(x => x.Priority).Where(x => x.Type == type).ToList();
}