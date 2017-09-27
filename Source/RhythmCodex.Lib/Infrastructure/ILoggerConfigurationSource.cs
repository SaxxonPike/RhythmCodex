namespace RhythmCodex.Infrastructure
{
    public interface ILoggerConfigurationSource
    {
        LoggerVerbosityLevel VerbosityLevel { get; set; }
    }
}