/*
 * These implementations are based on http://www.earlevel.com/main/2011/01/02/biquad-formulas/
 */

using System;
using JetBrains.Annotations;

namespace RhythmCodex.Plugin.CSCore.Lib.DSP;

/// <summary>
/// Used to apply a highpass-filter to a signal.
/// </summary>
[PublicAPI]
public class HighpassFilter : BiQuad
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HighpassFilter"/> class.
    /// </summary>
    /// <param name="sampleRate">The sample rate.</param>
    /// <param name="frequency">The filter's corner frequency.</param>
    public HighpassFilter(double sampleRate, double frequency) 
        : base(sampleRate, frequency)
    {
    }

    /// <summary>
    /// Calculates all coefficients.
    /// </summary>
    protected override void CalculateBiQuadCoefficients()
    {
        var k = Math.Tan(Math.PI * Frequency / SampleRate);
        var norm = 1 / (1 + k / Q + k * k);
        A0 = 1 * norm;
        A1 = -2 * A0;
        A2 = A0;
        B1 = 2 * (k * k - 1) * norm;
        B2 = (1 - k / Q + k * k) * norm;
    }
}