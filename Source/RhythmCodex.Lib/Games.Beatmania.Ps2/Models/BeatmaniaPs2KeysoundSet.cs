using System.Collections.Generic;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Games.Beatmania.Ps2.Models;

[Model]
public class BeatmaniaPs2KeysoundSet
{
    public List<BeatmaniaPs2Keysound> Keysounds { get; set; } = [];
}