using System.Collections.Generic;

namespace RhythmCodex.Sounds.Providers
{
    public interface IFilterContext
    {
        IList<float> Filter(IList<float> data);
    }
}