using System.Collections.Generic;

namespace RhythmCodex.Iso.Model
{
    public class CueInfo
    {
        public string BinFile { get; set; }
        public IList<CueTrack> Tracks { get; set; }
    }
}