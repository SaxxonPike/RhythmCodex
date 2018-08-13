namespace RhythmCodex.Audio.Models
{
    public interface IRiffChunk
    {
        string Id { get; }
        byte[] Data { get; }
    }
}