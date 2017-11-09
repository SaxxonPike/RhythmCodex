namespace RhythmCodex.Djmain.Model
{
    public interface IDjmainSampleInfo
    {
        byte Channel { get; }
        byte Flags { get; }
        ushort Frequency { get; }
        uint Offset { get; }
        byte Panning { get; }
        byte ReverbVolume { get; }
        byte SampleType { get; }
        byte Volume { get; }
    }
}