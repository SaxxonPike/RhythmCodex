using System.Collections.Generic;

namespace RhythmCodex.FileSystems.Cue.Model;

public class CueFile
{
    public List<CueTrack> Tracks { get; set; } = [];
    public List<List<string>> ExtraLines { get; set; } = [];
}