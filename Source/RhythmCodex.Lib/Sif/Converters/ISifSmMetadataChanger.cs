using System.Collections.Generic;
using RhythmCodex.Sif.Models;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Sif.Converters;

public interface ISifSmMetadataChanger
{
    void Apply(ICollection<Command> commands, SifInfo sif);
}