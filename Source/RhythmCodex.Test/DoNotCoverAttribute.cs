using System.Diagnostics.CodeAnalysis;

namespace RhythmCodex;

/// <summary>
/// Indicates that a test or tests should not be run during coverage.
/// </summary>
[ExcludeFromCodeCoverage]
public class DoNotCoverAttribute : Attribute
{
}