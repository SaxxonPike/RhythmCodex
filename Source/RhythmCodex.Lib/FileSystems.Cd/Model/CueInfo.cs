using System.Collections.Generic;

namespace RhythmCodex.FileSystems.Cd.Model;

public class CueInfo
{
    public string? BinFile { get; set; }
    public List<CueTrack> Tracks { get; set; } = [];
}