namespace RhythmCodex.Archs.Firebeat.Models;

public class FirebeatChartEvent
{
    public int Tick { get; set; }
    public byte Type { get; set; }
    public byte Player { get; set; }
    public ushort Data { get; set; }
}