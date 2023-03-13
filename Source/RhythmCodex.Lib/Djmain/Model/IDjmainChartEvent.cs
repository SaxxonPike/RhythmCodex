namespace RhythmCodex.Djmain.Model;

public interface IDjmainChartEvent
{
    ushort Offset { get; set; }
    byte Param0 { get; set; }
    byte Param1 { get; set; }
}