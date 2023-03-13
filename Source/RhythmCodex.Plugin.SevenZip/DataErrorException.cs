using System;

namespace RhythmCodex.Plugin.SevenZip;

/// <summary>
/// The exception that is thrown when an error in input stream occurs during decoding.
/// </summary>
class DataErrorException : ApplicationException
{
    public DataErrorException(): base("Data Error") { }
}