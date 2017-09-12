namespace RhythmCodex.Infrastructure
{
    /// <inheritdoc />
    [Model]
    public class FileRoute : IFileRoute
    {
        /// <inheritdoc />
        public string Name { get; set; }
        
        /// <inheritdoc />
        public byte[] Data { get; set; }
    }
}