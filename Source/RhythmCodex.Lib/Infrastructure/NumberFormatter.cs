﻿using System.Globalization;

namespace RhythmCodex.Infrastructure
{
    /// <inheritdoc />
    [Service]
    public class NumberFormatter : INumberFormatter
    {
        /// <inheritdoc />
        public string Format(BigRational value, int places) =>
            ((decimal) value).ToString($"F{places}", CultureInfo.InvariantCulture);
    }
}