using System.Collections.Generic;
using Numerics;
using RhythmCodex.Attributes;

namespace RhythmCodex.Charting
{
    public interface IChart
    {
        string this[string key] { get; set; }
        BigRational? this[NumericData type] { get; set; }
        bool? this[FlagData type] { get;set; }
        IList<IEvent> Events { get; }
    }
}
