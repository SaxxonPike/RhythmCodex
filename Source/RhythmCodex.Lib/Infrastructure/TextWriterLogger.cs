using System.IO;

namespace RhythmCodex.Infrastructure
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TextWriterLogger : ILogger
    {
        private readonly TextWriter _writer;
        private readonly ILoggerConfigurationSource _loggerConfigurationSource;

        public TextWriterLogger(TextWriter writer, ILoggerConfigurationSource loggerConfigurationSource)
        {
            _writer = writer;
            _loggerConfigurationSource = loggerConfigurationSource;
        }

        public void Debug(string message)
        {
            if (_loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Debug)
                _writer.WriteLine($"[debug]  {message}");
        }

        public void Info(string message)
        {
            if (_loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Info)
                _writer.WriteLine($"[info]   {message}");
        }

        public void Warning(string message)
        {
            if (_loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Warning)
                _writer.WriteLine($"[warn]   {message}");
        }

        public void Error(string message)
        {
            if (_loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Error)
                _writer.WriteLine($"[error]  {message}");
        }
    }
}