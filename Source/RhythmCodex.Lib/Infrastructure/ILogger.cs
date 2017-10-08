namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// An interface for writing to an application log.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log debug info that is not otherwise particularly useful to the user.
        /// </summary>
        void Debug(string message);
        
        /// <summary>
        /// Log information that is useful to the user but need not be acted upon.
        /// </summary>
        void Info(string message);
        
        /// <summary>
        /// Log a warning to inform the user there may be a problem which does not halt execution.
        /// </summary>
        void Warning(string message);
        
        /// <summary>
        /// Log an error to inform the user execution could not continue for some reason.
        /// </summary>
        void Error(string message);
    }
}