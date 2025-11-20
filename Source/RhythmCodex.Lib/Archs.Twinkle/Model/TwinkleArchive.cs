using System.Collections.Generic;
using RhythmCodex.Charts.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Archs.Twinkle.Model;

public class TwinkleArchive
{
    public int Id { get; set; }
    public List<Chart> Charts { get; set; } = [];
    public List<Sound> Samples { get; set; } = [];
}