using System.Diagnostics;

namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// An in-memory logger configuration source.
    /// </summary>
    [Service]
    public class LoggerConfigurationSource : ILoggerConfigurationSource
    {
        /// <summary>
        /// Create an in-memory logger configuration source.
        /// </summary>
        public LoggerConfigurationSource()
        {
            // Default to debug while debugger is running.
            VerbosityLevel = Debugger.IsAttached 
                ? LoggerVerbosityLevel.Debug 
                : LoggerVerbosityLevel.Info;
        }
        
        /// <inheritdoc/>
        public LoggerVerbosityLevel VerbosityLevel { get; set; }
    }
}