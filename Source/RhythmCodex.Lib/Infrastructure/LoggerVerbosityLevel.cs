namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// Filter level of the logger output.
    /// </summary>
    public enum LoggerVerbosityLevel
    {
        /// <summary>
        /// The most verbose level. The logger will include information not useful for end users.
        /// </summary>
        Debug,
        
        /// <summary>
        /// The default level. The logger will include non-urgent information that is useful for end users.
        /// </summary>
        Info,
        
        /// <summary>
        /// The logger will include information about potential issues that otherwise don't stop execution.
        /// </summary>
        Warning,
        
        /// <summary>
        /// The logger will include information about errors that stop execution.
        /// </summary>
        Error
    }
}