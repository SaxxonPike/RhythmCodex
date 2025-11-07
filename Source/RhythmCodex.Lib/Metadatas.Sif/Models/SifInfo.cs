using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Metadatas.Sif.Models;

[Model]
public class SifInfo
{
    public Dictionary<string, string> KeyValues { get; set; }
}