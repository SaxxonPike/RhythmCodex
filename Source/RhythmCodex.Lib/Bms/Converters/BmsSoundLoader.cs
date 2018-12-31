using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Bms.Converters
{
    [Service]
    public class BmsSoundLoader : IBmsSoundLoader
    {
        public IList<ISound> Load(IChart chart)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IBmsSoundLoader
    {
        IList<ISound> Load(IChart chart);
    }
}