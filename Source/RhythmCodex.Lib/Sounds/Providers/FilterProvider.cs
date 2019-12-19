using System.Collections.Generic;
using System.Linq;
using RhythmCodex.IoC;

namespace RhythmCodex.Sounds.Providers
{
    [Service]
    public class FilterProvider : IFilterProvider
    {
        private readonly IList<IFilter> _filters;

        public FilterProvider(IList<IFilter> filters)
        {
            _filters = filters;
        }
        
        public IEnumerable<IFilter> Get(FilterType type) => 
            _filters.OrderByDescending(x => x.Priority).Where(x => x.Type == type).ToList();
    }
}