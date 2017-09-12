namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// A routable file.
    /// </summary>
    public interface IFileRoute : IRoutable
    {
        /// <summary>
        /// Raw file data.
        /// </summary>
        byte[] Data { get; }
    }
}