using System.Collections.Generic;
using RhythmCodex.Metadatas.Sif.Models;

namespace RhythmCodex.Metadatas.Sif.Converters;

public interface ITextSifDecoder
{
    SifInfo Decode(IEnumerable<string> lines);
}