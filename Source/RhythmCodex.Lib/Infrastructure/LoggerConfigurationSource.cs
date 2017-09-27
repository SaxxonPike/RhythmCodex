namespace RhythmCodex.Infrastructure
{
    [Service]
    public class LoggerConfigurationSource : ILoggerConfigurationSource
    {
        public LoggerVerbosityLevel VerbosityLevel { get; set; }
    }
}