namespace RhythmCodex.Vtddd.Models;

public class VtdddStep
{
    public required int Panel { get; set; }
    public required int Player { get; set; }
    public required bool Hold { get; set; }
    public required int? Show { get; set; }
    public required int? Target { get; set; }
    public required int? End { get; set; }
}