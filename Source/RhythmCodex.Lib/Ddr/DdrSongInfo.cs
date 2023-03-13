using System.Collections.Generic;

namespace RhythmCodex.Ddr;

public class DdrSongInfo
{
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Artist { get; set; }
    public IList<int> Bpms { get; set; }
    public IDictionary<string, int> Levels { get; set; }
}