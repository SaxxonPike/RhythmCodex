using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Models;

[Model]
public class Ddr573Image
{
    public IDictionary<int, byte[]> Modules { get; set; }
}