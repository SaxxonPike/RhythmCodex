using System.Collections.Generic;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    public interface IVagSplitter
    {
        IList<ISample> Split(VagChunk chunk);
    }
}