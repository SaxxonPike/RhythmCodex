namespace RhythmCodex.Infrastructure;


public class TextWriterLogger(IConsole console, ILoggerConfigurationSource loggerConfigurationSource)
    : ILogger
{
    public void Debug(string message)
    {
        if (loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Debug)
            console.WriteLine($"[debug]  {message}");
    }

    public void Info(string message)
    {
        if (loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Info)
            console.WriteLine($"[info]   {message}");
    }

    public void Warning(string message)
    {
        if (loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Warning)
            console.WriteLine($"[warn]   {message}");
    }

    public void Error(string message)
    {
        if (loggerConfigurationSource.VerbosityLevel <= LoggerVerbosityLevel.Error)
            console.WriteLine($"[error]  {message}");
    }
}