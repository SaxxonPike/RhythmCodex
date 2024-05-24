using System.Collections.Generic;

namespace RhythmCodex.Cd.Model;

public class CueInfo
{
    public string? BinFile { get; set; }
    public List<CueTrack> Tracks { get; set; } = [];
}