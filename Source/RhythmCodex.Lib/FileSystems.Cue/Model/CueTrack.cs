using System.Collections.Generic;

namespace RhythmCodex.FileSystems.Cue.Model;

public class CueTrack
{
    public int Number { get; set; }
    public Dictionary<int, int> Indices { get; set; } = [];
    public string? Type { get; set; }
    public int Pregap { get; set; }
    public int Postgap { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public List<List<string>> ExtraLines { get; set; } = [];
    public int StoredBytesPerSector { get; set; }
}