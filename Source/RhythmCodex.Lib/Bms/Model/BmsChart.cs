using System.Collections.Generic;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Bms.Model
{
    public class BmsChart
    {
        public Chart Chart { get; set; }
        public IDictionary<int, string> SoundMap { get; set; }
    }
}