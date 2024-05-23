namespace RhythmCodex.Iso.Model;

public class IsoDateTime
{
    public int YearsSince1900 { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Second { get; set; }
    public int Offset15 { get; set; }
}