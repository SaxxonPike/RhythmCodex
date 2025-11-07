using System.Collections.Generic;

namespace RhythmCodex.Sounds.Filter.Providers;

public interface IFilterProvider
{
    IEnumerable<IFilter> Get(FilterType type);
}