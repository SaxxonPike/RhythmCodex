using System.Collections.Generic;
using RhythmCodex.Games.Stepmania.Model;
using RhythmCodex.Metadatas.Sif.Models;

namespace RhythmCodex.Metadatas.Sif.Converters;

public interface ISifSmMetadataChanger
{
    void Apply(ICollection<Command> commands, SifInfo sif);
}