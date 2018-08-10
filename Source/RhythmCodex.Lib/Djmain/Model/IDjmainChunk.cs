namespace RhythmCodex.Djmain.Model
{
    public interface IDjmainChunk
    {
        DjmainChunkFormat Format { get; }
        byte[] Data { get; }
        int Id { get; }
    }
}