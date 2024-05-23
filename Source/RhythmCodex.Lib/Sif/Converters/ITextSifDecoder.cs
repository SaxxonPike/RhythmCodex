using System.Collections.Generic;
using RhythmCodex.Sif.Models;

namespace RhythmCodex.Sif.Converters;

public interface ITextSifDecoder
{
    SifInfo Decode(IEnumerable<string> lines);
}