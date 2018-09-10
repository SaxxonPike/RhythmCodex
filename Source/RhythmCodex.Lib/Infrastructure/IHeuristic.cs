namespace RhythmCodex.Infrastructure
{
    public interface IHeuristic
    {
        string Description { get; }
        string FileExtension { get; }
        bool IsMatch(byte[] data);
    }
}