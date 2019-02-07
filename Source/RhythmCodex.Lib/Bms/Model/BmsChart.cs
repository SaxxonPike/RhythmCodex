using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Charting.Models;

namespace RhythmCodex.Bms.Model
{
    public class BmsChart
    {
        public IChart Chart { get; set; }
        public IDictionary<int, string> SoundMap { get; set; }
    }
}