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
    }
}