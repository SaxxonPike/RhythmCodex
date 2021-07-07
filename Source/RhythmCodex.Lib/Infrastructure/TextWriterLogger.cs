namespace RhythmCodex.Infrastructure
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TextWriterLogger : ILogger
    {
        private readonly IConsole _console;
        private readonly ILoggerConfigurationSource _loggerConfigurationSource;

        public TextWriterLogger(IConsole console, ILoggerConfigurationSource loggerConfigurationSource)
        {
            _console = console;
            _loggerConfigurationSource = loggerConfigurationSource;
        }

        public void Debug(string message)
        {
            if (_loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Debug)
                _console.WriteLine($"[debug]  {message}");
        }

        public void Info(string message)
        {
            if (_loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Info)
                _console.WriteLine($"[info]   {message}");
        }

        public void Warning(string message)
        {
            if (_loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Warning)
                _console.WriteLine($"[warn]   {message}");
        }

        public void Error(string message)
        {
            if (_loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Error)
                _console.WriteLine($"[error]  {message}");
        }
    }
}