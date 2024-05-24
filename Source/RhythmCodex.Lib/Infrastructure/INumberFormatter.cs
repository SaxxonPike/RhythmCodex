namespace RhythmCodex.Infrastructure;

/// <summary>
///     A culture-invariant number formatter.
/// </summary>
public interface INumberFormatter
{
    /// <summary>
    ///     Format a value to the specified number of places, fixed point.
    /// </summary>
    string Format(BigRational value, int places);
}