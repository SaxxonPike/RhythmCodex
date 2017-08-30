namespace RhythmCodex.Djmain.Model
{
    public class DjmainChartEvent : IDjmainChartEvent
    {
        public ushort Offset { get; set; }
        public byte Param0 { get; set; }
        public byte Param1 { get; set; }
    }
}
