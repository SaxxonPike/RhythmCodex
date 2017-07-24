using System.Collections.Generic;
using Numerics;

namespace RhythmCodex.Charting
{
    public interface IChart
    {
        string this[StringData type] { get; set; }
        BigRational this[NumericData type] { get; set; }
        bool this[FlagData type] { get;set; }
        List<IEvent> Events { get; }
    }
}
