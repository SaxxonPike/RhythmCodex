using System.Collections.Generic;
using Numerics;
using RhythmCodex.Attributes;

namespace RhythmCodex.Audio
{
    public interface ISound
    {
        string this[string key] { get; set; }
        BigRational? this[NumericData type] { get; set; }
        bool? this[FlagData type] { get; set; }
        IList<ISample> Samples { get; set; }
    }
}