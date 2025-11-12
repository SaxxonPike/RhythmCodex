/*
 * These implementations are based on http://www.earlevel.com/main/2011/01/02/biquad-formulas/
 */

using System;
using RhythmCodex.Sounds.Filter.Providers;

namespace RhythmCodex.Plugin.CSCore.Lib.DSP;

/// <summary>
/// Represents a biquad-filter.
/// </summary>
public abstract class BiQuad : IFilterContext
{
    /// <summary>
    /// The a0 value.
    /// </summary>
    protected double A0;
    /// <summary>
    /// The a1 value.
    /// </summary>
    protected double A1;
    /// <summary>
    /// The a2 value.
    /// </summary>
    protected double A2;
    /// <summary>
    /// The b1 value.
    /// </summary>
    protected double B1;
    /// <summary>
    /// The b2 value.
    /// </summary>
    protected double B2;
    /// <summary>
    /// The q value.
    /// </summary>
    private double _q;
    /// <summary>
    /// The gain value in dB.
    /// </summary>
    private double _gainDb;
    /// <summary>
    /// The z1 value.
    /// </summary>
    protected double Z1;
    /// <summary>
    /// The z2 value.
    /// </summary>
    protected double Z2;

    private double _frequency;

    /// <summary>
    /// Gets or sets the frequency.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">value;The samplerate has to be bigger than 2 * frequency.</exception>
    public double Frequency
    {
        get => _frequency;
        set
        {
            if (SampleRate < value * 2)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The samplerate has to be bigger than 2 * frequency.");
            }
            _frequency = value;
            CalculateBiQuadCoefficients();
        }
    }

    /// <summary>
    /// Gets the sample rate.
    /// </summary>
    public double SampleRate { get; }

    /// <summary>
    /// The q value.
    /// </summary>
    public double Q
    {
        get => _q;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            _q = value;
            CalculateBiQuadCoefficients();
        }
    }

    /// <summary>
    /// Gets or sets the gain value in dB.
    /// </summary>
    public double GainDB
    {
        get => _gainDb;
        set
        {
            _gainDb = value;
            CalculateBiQuadCoefficients();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiQuad"/> class.
    /// </summary>
    /// <param name="sampleRate">The sample rate.</param>
    /// <param name="frequency">The frequency.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// sampleRate
    /// or
    /// frequency
    /// or
    /// q
    /// </exception>
    protected BiQuad(double sampleRate, double frequency)
        : this(sampleRate, frequency, 1.0 / Math.Sqrt(2))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BiQuad"/> class.
    /// </summary>
    /// <param name="sampleRate">The sample rate.</param>
    /// <param name="frequency">The frequency.</param>
    /// <param name="q">The q.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// sampleRate
    /// or
    /// frequency
    /// or
    /// q
    /// </exception>
    protected BiQuad(double sampleRate, double frequency, double q)
    {
        if (sampleRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(sampleRate));
        if (frequency <= 0)
            throw new ArgumentOutOfRangeException(nameof(frequency));
        if (q <= 0)
            throw new ArgumentOutOfRangeException(nameof(q));
        SampleRate = sampleRate;
        Frequency = frequency;
        Q = q;
        GainDB = 6;
    }

    /// <summary>
    /// Processes multiple <paramref name="input"/> samples.
    /// </summary>
    /// <param name="input">The input samples to process.</param>
    /// <remarks>The result of the calculation gets stored within the <paramref name="input"/> array.</remarks>
    public float[] Process(ReadOnlySpan<float> input)
    {
        var result = new float[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            var iv = input[i];
            var o = iv * A0 + Z1;
            Z1 = iv * A1 + Z2 - B1 * o;
            Z2 = iv * A2 - B2 * o;
            result[i] = (float)o;
        }
        return result;
    }

    /// <summary>
    /// Calculates all coefficients.
    /// </summary>
    protected abstract void CalculateBiQuadCoefficients();

    float[] IFilterContext.Filter(ReadOnlySpan<float> data)
    {
        return Process(data);
    }
}