using System.Collections.Generic;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Converters;

public interface IVagSplitter
{
    List<Sample> Split(VagChunk? chunk);
}