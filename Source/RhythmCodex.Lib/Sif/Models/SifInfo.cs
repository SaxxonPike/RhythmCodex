using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Sif.Models;

[Model]
public class SifInfo
{
    public IDictionary<string, string> KeyValues { get; set; }
}