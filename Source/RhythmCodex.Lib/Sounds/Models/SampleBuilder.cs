using System;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Sounds.Models;

/// <summary>
/// Allows for building <see cref="Sample"/> objects by individual float or span of floats.
/// </summary>
[PublicAPI]
public sealed class SampleBuilder : Metadata, IDisposable
{
    /// <summary>
    /// Copies <see cref="Sample"/> data and metadata to a new builder.
    /// </summary>
    public static SampleBuilder FromSample(Sample sample)
    {
        var builder = new SampleBuilder();
        builder.Append(sample.Data.Span);
        builder.CloneMetadataFrom(sample);
        return builder;
    }

    /// <summary>
    /// True if Dispose has been invoked.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Underlying stream that holds the data.
    /// </summary>
    private readonly MemoryStream _stream = new();

    /// <summary>
    /// Retrieves sample data.
    /// </summary>
    public Span<float> AsSpan()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return MemoryMarshal
            .Cast<byte, float>(_stream.GetBuffer().AsSpan(0, (int)_stream.Length));
    }

    /// <summary>
    /// Retrieves sample data.
    /// </summary>
    public Span<float> AsSpan(int offset)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return MemoryMarshal
            .Cast<byte, float>(_stream.GetBuffer().AsSpan(0, (int)_stream.Length))[offset..];
    }

    /// <summary>
    /// Retrieves sample data.
    /// </summary>
    public Span<float> AsSpan(int offset, int length)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return MemoryMarshal
            .Cast<byte, float>(_stream.GetBuffer().AsSpan(0, (int)_stream.Length))
            .Slice(offset, length);
    }

    /// <summary>
    /// Replaces sample data with the specified value.
    /// </summary>
    public void Fill(float value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (value == 0)
            AsSpan().Clear();
        else
            AsSpan().Fill(value);
    }

    /// <summary>
    /// Appends one value to the end of the sample data.
    /// </summary>
    public void Append(float value)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        Span<float> span = stackalloc float[1];
        span[0] = value;
        _stream.Write(MemoryMarshal.Cast<float, byte>(span));
    }

    /// <summary>
    /// Appends a span of values to the end of the sample data.
    /// </summary>
    public void Append(ReadOnlySpan<float> data)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _stream.Write(MemoryMarshal.Cast<float, byte>(data));
    }

    /// <summary>
    /// Clears sample data.
    /// </summary>
    public void Clear()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _stream.Position = 0;
        _stream.SetLength(0);
    }

    /// <summary>
    /// Sets the length of the sample data in floats.
    /// </summary>
    public void SetLength(int length)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _stream.SetLength(length * sizeof(float));
    }

    /// <summary>
    /// Length of the sample data in floats.
    /// </summary>
    public int Length
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            return (int)(_stream.Length / sizeof(float));
        }
    }

    /// <summary>
    /// Copies the sample data and metadata to a new <see cref="Sample"/>.
    /// </summary>
    /// <param name="maxLength">
    /// Maximum number of samples to copy.
    /// </param>
    public Sample ToSample(int? maxLength = null)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var result = new Sample
        {
            Data = (maxLength == null ? AsSpan() : AsSpan(0, maxLength.Value)).ToArray()
        };

        result.CloneMetadataFrom(this);
        return result;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _stream.SetLength(0);
        _stream.Dispose();
    }
}