namespace RhythmCodex.Vtddd.Models
{
    public class VtdddStep
    {
        public int Panel { get; set; }
        public int Player { get; set; }
        public bool Hold { get; set; }
        public int? Show { get; set; }
        public int? Target { get; set; }
        public int? End { get; set; }
    }
}