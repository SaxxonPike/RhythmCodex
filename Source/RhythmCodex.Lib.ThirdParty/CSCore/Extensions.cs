using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace CSCore
{
    /// <summary>
    ///     Provides a few basic extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Writes all audio data of the <paramref name="waveSource"/> to a stream. In comparison to the <see cref="WriteToWaveStream"/> method, 
        /// the <see cref="WriteToStream"/> method won't encode the provided audio to any particular format. No wav, aiff,... header won't be included.
        /// </summary>
        /// <param name="waveSource">The waveSource which provides the audio data to write to the <paramref name="stream"/>.</param>
        /// <param name="stream">The <see cref="Stream"/> to store the audio data in.</param>
        /// <exception cref="System.ArgumentNullException">
        /// waveSource
        /// or
        /// stream
        /// </exception>
        /// <exception cref="System.ArgumentException">Stream is not writeable.;stream</exception>
        public static void WriteToStream(this IWaveSource waveSource, Stream stream)
        {
            if (waveSource == null)
                throw new ArgumentNullException(nameof(waveSource));
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite)
                throw new ArgumentException("Stream is not writeable.", nameof(stream));


            var buffer = new byte[waveSource.WaveFormat.BytesPerSecond];
            int read;
            while ((read = waveSource.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, read);
            }
        }

        /// <summary>
        /// Checks the length of an array.
        /// </summary>
        /// <typeparam name="T">Type of the array.</typeparam>
        /// <param name="inst">The array to check. This parameter can be null.</param>
        /// <param name="size">The target length of the array.</param>
        /// <param name="exactSize">A value which indicates whether the length of the array has to fit exactly the specified <paramref name="size"/>.</param>
        /// <returns>Array which fits the specified requirements. Note that if a new array got created, the content of the old array won't get copied to the return value.</returns>
        public static T[] CheckBuffer<T>(this T[] inst, long size, bool exactSize = false)
        {
            if (inst == null || (!exactSize && inst.Length < size) || (exactSize && inst.Length != size))
                return new T[size];
            return inst;
        }

        internal static byte[] ReadBytes(this IWaveSource waveSource, int count)
        {
            if (waveSource == null)
                throw new ArgumentNullException(nameof(waveSource));
            count -= (count % waveSource.WaveFormat.BlockAlign);
            if(count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            byte[] buffer = new byte[count];
            int read = waveSource.Read(buffer, 0, buffer.Length);
            if(read < count)
                Array.Resize(ref buffer, read);
            return buffer;
        }

        internal static bool IsClosed(this Stream stream)
        {
            return !stream.CanRead && !stream.CanWrite;
        }

        internal static bool IsEndOfStream(this Stream stream)
        {
            return stream.Position == stream.Length;
        }

        //copied from http://stackoverflow.com/questions/1436190/c-sharp-get-and-set-the-high-order-word-of-an-integer
        internal static int LowWord(this int number)
        {
            return number & 0x0000FFFF;
        }

        internal static int LowWord(this int number, int newValue)
        {
            return (int) ((number & 0xFFFF0000) + (newValue & 0x0000FFFF));
        }

        internal static int HighWord(this int number)
        {
            return (int) (number & 0xFFFF0000);
        }

        internal static int HighWord(this int number, int newValue)
        {
            return (number & 0x0000FFFF) + (newValue << 16);
        }

        internal static uint LowWord(this uint number)
        {
            return number & 0x0000FFFF;
        }

        internal static uint LowWord(this uint number, int newValue)
        {
            return (uint) ((number & 0xFFFF0000) + (newValue & 0x0000FFFF));
        }

        internal static uint HighWord(this uint number)
        {
            return number & 0xFFFF0000;
        }

        internal static uint HighWord(this uint number, int newValue)
        {
            return (uint) ((number & 0x0000FFFF) + (newValue << 16));
        }
        //--

        internal static Guid GetGuid(this Object obj)
        {
            return obj.GetType().GUID;
        }

        internal static void WaitForExit(this Thread thread)
        {
            if (thread == null)
                return;
            if (thread == Thread.CurrentThread)
                throw new InvalidOperationException("Deadlock detected.");

            thread.Join();
        }

        internal static bool WaitForExit(this Thread thread, int timeout)
        {
            if (thread == null)
                return true;
            if (thread == Thread.CurrentThread)
                throw new InvalidOperationException("Deadlock detected.");

            return thread.Join(timeout);
        }

// ReSharper disable once InconsistentNaming
        internal static bool IsPCM(this WaveFormat waveFormat)
        {
            if (waveFormat == null)
                throw new ArgumentNullException(nameof(waveFormat));
            if (waveFormat is WaveFormatExtensible)
                return ((WaveFormatExtensible) waveFormat).SubFormat == AudioSubTypes.Pcm;
            return waveFormat.WaveFormatTag == AudioEncoding.Pcm;
        }

        internal static bool IsIeeeFloat(this WaveFormat waveFormat)
        {
            if (waveFormat == null)
                throw new ArgumentNullException(nameof(waveFormat));
            if (waveFormat is WaveFormatExtensible)
                return ((WaveFormatExtensible) waveFormat).SubFormat == AudioSubTypes.IeeeFloat;
            return waveFormat.WaveFormatTag == AudioEncoding.IeeeFloat;
        }

        internal static AudioEncoding GetWaveFormatTag(this WaveFormat waveFormat)
        {
            if (waveFormat is WaveFormatExtensible)
                return AudioSubTypes.EncodingFromSubType(((WaveFormatExtensible) waveFormat).SubFormat);

            return waveFormat.WaveFormatTag;
        }

        //copied from http://stackoverflow.com/questions/9927590/can-i-set-a-value-on-a-struct-through-reflection-without-boxing
        internal static void SetValueForValueType<T>(this FieldInfo field, ref T item, object value) where T : struct
        {
            field.SetValueDirect(__makeref(item), value);
        }
    }
}
