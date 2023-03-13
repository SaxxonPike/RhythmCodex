namespace RhythmCodex.Riff.Models;

public class RiffChunk : IRiffChunk
{
    public string Id { get; set; }
    public byte[] Data { get; set; }
}