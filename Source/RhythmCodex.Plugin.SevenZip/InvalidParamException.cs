using System;

namespace RhythmCodex.Plugin.SevenZip;

/// <summary>
/// The exception that is thrown when the value of an argument is outside the allowable range.
/// </summary>
class InvalidParamException : ApplicationException
{
    public InvalidParamException(): base("Invalid Parameter") { }
}