namespace RhythmCodex.Archs.Firebeat.Models;

public class FirebeatBeatmaniaChartHeader
{
    public short MaxNoteCount1p { get; set; }
    public short MaxNoteCount2p { get; set; }
    public short MinNoteCount1p { get; set; }
    public short MinNoteCount2p { get; set; }
    public short MaxBpm { get; set; }
    public short MinBpm { get; set; }
    public short Unk0C { get; set; }
    public FirebeatBeatmaniaChartHeaderFlags0E Flags0E { get; set; }
    public FirebeatBeatmaniaChartHeaderFlags10 Flags10 { get; set; }
}