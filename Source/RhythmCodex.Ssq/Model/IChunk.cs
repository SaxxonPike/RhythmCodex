namespace RhythmCodex.Ssq.Model
{
    public interface IChunk
    {
        byte[] Data { get; }
        int Parameter0 { get; }
        int Parameter1 { get; }
    }
}