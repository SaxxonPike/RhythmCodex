namespace RhythmCodex.Cli
{
    /// <summary>
    /// The main application container.
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// Begins executing this instance of the application container.
        /// </summary>
        void Run(string[] args);
    }
}