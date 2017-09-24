using System;

namespace RhythmCodex.Infrastructure
{
    /// <summary>
    ///     This type of exception is used for all non-system exceptions within the base library
    ///     and supplemental libraries.
    /// </summary>
    public class RhythmCodexException : Exception
    {
        /// <inheritdoc />
        public RhythmCodexException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public RhythmCodexException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}