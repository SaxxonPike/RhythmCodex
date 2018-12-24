using System;

namespace CSCore.Tags.ID3
{
    /// <summary>
    /// Exception class for all ID3-Tag related Exceptions.
    /// </summary>
    [Serializable]
    public class ID3Exception : Exception
    {
        internal ID3Exception(string message, params object[] args)
            : this(string.Format(message, args))
        {
        }

        internal ID3Exception(string message)
            : base(message)
        {
        }

        internal ID3Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}