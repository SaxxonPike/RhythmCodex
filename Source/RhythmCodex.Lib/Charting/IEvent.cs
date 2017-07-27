using Numerics;

namespace RhythmCodex.Charting
{
    public interface IEvent
    {
        string this[string key] { get; set; }
        BigRational? this[NumericData type] { get; set; }
        bool? this[FlagData type] { get; set; }
    }
}
