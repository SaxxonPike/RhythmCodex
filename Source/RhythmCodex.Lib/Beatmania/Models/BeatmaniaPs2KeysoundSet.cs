using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Beatmania.Models;

[Model]
public class BeatmaniaPs2KeysoundSet
{
    public List<BeatmaniaPs2Keysound> Keysounds { get; set; } = [];
}