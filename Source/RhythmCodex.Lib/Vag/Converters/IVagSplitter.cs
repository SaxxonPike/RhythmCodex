using System.Collections.Generic;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters;

public interface IVagSplitter
{
    List<Sample> Split(VagChunk? chunk);
}