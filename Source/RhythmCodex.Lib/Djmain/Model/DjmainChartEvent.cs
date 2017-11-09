using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Model
{
    [Model]
    public class DjmainChartEvent : IDjmainChartEvent
    {
        public ushort Offset { get; set; }
        public byte Param0 { get; set; }
        public byte Param1 { get; set; }
    }
}