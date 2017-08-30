namespace RhythmCodex.Djmain.Model
{
    public class DjmainSampleInfo : IDjmainSampleInfo
    {
        public byte Channel { get; set; }
        public byte Flags { get; set; }
        public ushort Frequency { get; set; }
        public uint Offset { get; set; }
        public byte Panning { get; set; }
        public byte ReverbVolume { get; set; }
        public byte SampleType { get; set; }
        public byte Volume { get; set; }
    }
}
