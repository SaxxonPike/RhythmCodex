namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// A configuration source for the logger.
    /// </summary>
    public interface ILoggerConfigurationSource
    {
        /// <summary>
        /// Filter level of the log output.
        /// </summary>
        LoggerVerbosityLevel VerbosityLevel { get; set; }
    }
}