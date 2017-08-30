namespace RhythmCodex.Ssq.Model
{
    public interface IChunk
    {
        byte[] Data { get; }
        short Parameter0 { get; }
        short Parameter1 { get; }
    }
}