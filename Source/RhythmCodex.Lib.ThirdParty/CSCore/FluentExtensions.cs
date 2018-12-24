﻿using System;
using System.Diagnostics;
using CSCore.Streams;
using CSCore.Streams.SampleConverter;

namespace CSCore
{
    /// <summary>
    ///     Provides a basic fluent API for creating a source chain.
    /// </summary>
    public static class FluentExtensions
    {
        /// <summary>
        ///     Appends a source to an already existing source.
        /// </summary>
        /// <typeparam name="TInput">Input</typeparam>
        /// <typeparam name="TResult">Output</typeparam>
        /// <param name="input">Already existing source.</param>
        /// <param name="func">Function which appends the new source to the already existing source.</param>
        /// <returns>The return value of the <paramref name="func" /> delegate.</returns>
        public static TResult AppendSource<TInput, TResult>(this TInput input, Func<TInput, TResult> func)
            where TInput : IAudioSource
        {
            return func(input);
        }

        /// <summary>
        ///     Appends a source to an already existing source.
        /// </summary>
        /// <typeparam name="TInput">Input</typeparam>
        /// <typeparam name="TResult">Output</typeparam>
        /// <param name="input">Already existing source.</param>
        /// <param name="func">Function which appends the new source to the already existing source.</param>
        /// <param name="outputSource">Receives the return value.</param>
        /// <returns>The return value of the <paramref name="func" /> delegate.</returns>
        public static TResult AppendSource<TInput, TResult>(this TInput input, Func<TInput, TResult> func,
            out TResult outputSource)
            where TInput : IAudioSource
        {
            outputSource = func(input);
            return outputSource;
        }

        /// <summary>
        ///     Appends a new instance of the <see cref="LoopStream" /> class to the audio chain.
        /// </summary>
        /// <param name="input">The underlying <see cref="IWaveSource" /> which should be looped.</param>
        /// <returns>The new instance <see cref="LoopStream" /> instance.</returns>
        public static IWaveSource Loop(this IWaveSource input)
        {
            return new LoopStream(input) {EnableLoop = true};
        }

        /// <summary>
        ///     Converts a SampleSource to either a Pcm (8, 16, or 24 bit) or IeeeFloat (32 bit) WaveSource.
        /// </summary>
        /// <param name="sampleSource">Sample source to convert to a wave source.</param>
        /// <param name="bits">Bits per sample.</param>
        /// <returns>Wave source</returns>
        public static IWaveSource ToWaveSource(this ISampleSource sampleSource, int bits)
        {
            if (sampleSource == null)
                throw new ArgumentNullException(nameof(sampleSource));

            switch (bits)
            {
                case 8:
                    return new SampleToPcm8(sampleSource);
                case 16:
                    return new SampleToPcm16(sampleSource);
                case 24:
                    return new SampleToPcm24(sampleSource);
                case 32:
                    return new SampleToIeeeFloat32(sampleSource);
                default:
                    throw new ArgumentOutOfRangeException(nameof(bits), "Must be 8, 16, 24 or 32 bits.");
            }
        }

        /// <summary>
        ///     Converts a <see cref="IWaveSource"/> to IeeeFloat (32bit) <see cref="IWaveSource"/>.
        /// </summary>
        /// <param name="sampleSource">The <see cref="ISampleSource"/> to convert to a <see cref="IWaveSource"/>.</param>
        /// <returns>The <see cref="IWaveSource"/> wrapped around the specified <paramref name="sampleSource"/>.</returns>
        public static IWaveSource ToWaveSource(this ISampleSource sampleSource)
        {
            if (sampleSource == null)
                throw new ArgumentNullException(nameof(sampleSource));

            return new SampleToIeeeFloat32(sampleSource);
        }

        /// <summary>
        ///     Converts a <see cref="IWaveSource"/> to a <see cref="ISampleSource"/>.
        /// </summary>
        /// <param name="waveSource">The <see cref="IWaveSource"/> to convert to a <see cref="ISampleSource"/>.</param>
        /// <returns>The <see cref="ISampleSource"/> wrapped around the specified <paramref name="waveSource"/>.</returns>        
        public static ISampleSource ToSampleSource(this IWaveSource waveSource)
        {
            if (waveSource == null)
                throw new ArgumentNullException(nameof(waveSource));

            return WaveToSampleBase.CreateConverter(waveSource);
        }

        /// <summary>
        ///     Returns a thread-safe (synchronized) wrapper around the specified <typeparamref name="TAudioSource" /> object.
        /// </summary>
        /// <param name="audioSource">The <typeparamref name="TAudioSource" /> object to synchronize.</param>
        /// <typeparam name="TAudioSource">Type of the <paramref name="audioSource" /> argument.</typeparam>
        /// <typeparam name="T">The type of the data read by the Read method of the <paramref name="audioSource"/> method.</typeparam>
        /// <returns>A thread-safe wrapper around the specified <typeparamref name="TAudioSource" /> object.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="audioSource" /> is null.</exception>
        public static SynchronizedWaveSource<TAudioSource, T> Synchronized<TAudioSource, T>(this TAudioSource audioSource)
            where TAudioSource : class, IReadableAudioSource<T>
        {
            if (audioSource == null)
                throw new ArgumentNullException(nameof(audioSource));

            return new SynchronizedWaveSource<TAudioSource, T>(audioSource);
        }

        /// <summary>
        ///     Registers an <paramref name="action"/> to be executed on end of stream of the <paramref name="waveSource"/>.
        /// </summary>
        /// <param name="waveSource">The <see cref="IWaveSource"/> to be tracked.</param>
        /// <param name="action">The callback action with the <paramref name="waveSource"/> as parameter.</param>
        /// <returns>A wrapper around the <paramref name="waveSource"/>.</returns>
        public static IWaveSource OnEndOfStream(this IWaveSource waveSource, Action<IWaveSource> action)
        {
            return new EofTrackingWaveSource(waveSource, action);
        }

        /// <summary>
        ///     Registers an <paramref name="action"/> to be executed on end of stream of the <paramref name="sampleSource"/>.
        /// </summary>
        /// <param name="sampleSource">The <see cref="ISampleSource"/> to be tracked.</param>
        /// <param name="action">The callback action with the <paramref name="sampleSource"/> as parameter.</param>
        /// <returns>A wrapper around the <paramref name="sampleSource"/>.</returns>
        public static ISampleSource OnEndOfStream(this ISampleSource sampleSource, Action<ISampleSource> action)
        {
            return new EofTrackingSampleSource(sampleSource, action);
        }

        private class EofTrackingWaveSource : WaveAggregatorBase
        {
            private readonly Action<IWaveSource> _action;
            private bool _eofReached = false;

            public EofTrackingWaveSource(IWaveSource source, Action<IWaveSource> action)
                : base(source)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int read = base.Read(buffer, offset, count);
                if (read <= 0 && count - offset > WaveFormat.BlockAlign)
                {
                    if (!_eofReached)
                    {
                        _eofReached = true;
                        _action(BaseSource);
                    }
                }
                else
                {
                    _eofReached = false;
                }
                return read;
            }
        }

        private class EofTrackingSampleSource : SampleAggregatorBase
        {
            private readonly Action<ISampleSource> _action;
            private bool _eofReached = false;

            public EofTrackingSampleSource(ISampleSource source, Action<ISampleSource> action)
                : base(source)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
            }

            public override int Read(float[] buffer, int offset, int count)
            {
                int read = base.Read(buffer, offset, count);
                if (read <= 0 && offset - count > WaveFormat.Channels)
                {
                    if (!_eofReached)
                    {
                        _eofReached = true;
                        _action(BaseSource);
                    }
                }
                else
                {
                    _eofReached = false;
                }
                return read;
            }
        }
    }
}