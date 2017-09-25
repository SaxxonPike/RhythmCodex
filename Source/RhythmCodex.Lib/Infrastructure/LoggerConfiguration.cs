namespace RhythmCodex.Infrastructure
{
    [Model]
    public class LoggerConfiguration : ILoggerConfiguration
    {
        public LoggerVerbosityLevel VerbosityLevel { get; set; }
    }
}