using System.Collections.Generic;

namespace RhythmCodex.Cd.Model;

public class CueInfo
{
    public string? BinFile { get; set; }
    public IList<CueTrack> Tracks { get; set; } = [];
}