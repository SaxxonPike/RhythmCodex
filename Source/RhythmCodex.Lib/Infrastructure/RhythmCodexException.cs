using System;

namespace RhythmCodex.Infrastructure
{
    public class RhythmCodexException : Exception
    {
        public RhythmCodexException(string message) : base(message)
        {
        }
        
        public RhythmCodexException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
