using System.Collections.Generic;

namespace RhythmCodex.Sounds.Providers;

public interface IFilterProvider
{
    IEnumerable<IFilter> Get(FilterType type);
}