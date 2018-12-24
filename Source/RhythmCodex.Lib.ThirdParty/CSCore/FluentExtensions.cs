using System;

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