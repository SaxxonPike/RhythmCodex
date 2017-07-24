using Numerics;

namespace RhythmCodex.Charting
{
    public interface IEvent
    {
        string this[StringData type] { get; set; }
        BigRational this[NumericData type] { get; set; }
        bool this[FlagData type] { get; set; }
    }
}
