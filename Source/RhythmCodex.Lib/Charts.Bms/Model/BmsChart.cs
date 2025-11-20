using System.Collections.Generic;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Charts.Bms.Model;

public class BmsChart
{
    public Chart Chart { get; set; } = new();
    public Dictionary<int, string> SoundMap { get; set; } = new();
}