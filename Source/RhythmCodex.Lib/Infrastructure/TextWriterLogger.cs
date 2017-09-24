using System.IO;

namespace RhythmCodex.Infrastructure
{
    public class TextWriterLogger : ILogger
    {
        private readonly TextWriter _writer;

        public TextWriterLogger(TextWriter writer)
        {
            _writer = writer;
        }

        public void Debug(string message)
        {
            _writer.WriteLine($"[debug]  {message}");
        }

        public void Info(string message)
        {
            _writer.WriteLine($"[info]   {message}");
        }

        public void Warning(string message)
        {
            _writer.WriteLine($"[warn]   {message}");
        }

        public void Error(string message)
        {
            _writer.WriteLine($"[error]  {message}");
        }
    }
}