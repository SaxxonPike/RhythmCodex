using Numerics;
using RhythmCodex.Attributes;

namespace RhythmCodex.Charting
{
    public interface IMetadata
    {
        bool? this[FlagData type] { get; set; }
        BigRational? this[NumericData type] { get; set; }
        string this[string key] { get; set; }
    }
}