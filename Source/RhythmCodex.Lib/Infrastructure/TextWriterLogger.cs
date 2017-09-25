using System.IO;

namespace RhythmCodex.Infrastructure
{
    public class TextWriterLogger : ILogger
    {
        private readonly TextWriter _writer;
        private readonly ILoggerConfiguration _loggerConfiguration;

        public TextWriterLogger(TextWriter writer, ILoggerConfiguration loggerConfiguration)
        {
            _writer = writer;
            _loggerConfiguration = loggerConfiguration;
        }

        public void Debug(string message)
        {
            if (_loggerConfiguration.VerbosityLevel <= LoggerVerbosityLevel.Debug)
                _writer.WriteLine($"[debug]  {message}");
        }

        public void Info(string message)
        {
            if (_loggerConfiguration.VerbosityLevel <= LoggerVerbosityLevel.Info)
                _writer.WriteLine($"[info]   {message}");
        }

        public void Warning(string message)
        {
            if (_loggerConfiguration.VerbosityLevel <= LoggerVerbosityLevel.Warning)
                _writer.WriteLine($"[warn]   {message}");
        }

        public void Error(string message)
        {
            if (_loggerConfiguration.VerbosityLevel <= LoggerVerbosityLevel.Error)
                _writer.WriteLine($"[error]  {message}");
        }
    }
}