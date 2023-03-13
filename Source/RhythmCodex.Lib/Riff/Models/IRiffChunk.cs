namespace RhythmCodex.Riff.Models;

public interface IRiffChunk
{
    string Id { get; }
    byte[] Data { get; }
}