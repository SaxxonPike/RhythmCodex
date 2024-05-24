using System.Globalization;
using RhythmCodex.IoC;

namespace RhythmCodex.Infrastructure;

/// <inheritdoc />
[Service]
public class NumberFormatter : INumberFormatter
{
    /// <inheritdoc />
    public string Format(BigRational value, int places)
    {
        return ((decimal) value).ToString($"F{places}", CultureInfo.InvariantCulture);
    }
}