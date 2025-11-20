using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Charts.Sm.Model;

[Model]
public class Command
{
    public string Name { get; set; }
    public List<string> Values { get; set; } = [];
}