using System;
using System.IO;

namespace CSCore
{
    /// <summary>
    ///     Provides a few basic extensions.
    /// </summary>
    public static class Extensions
    {
        internal static byte[] ReadBytes(this IReadableAudioSource<byte> waveSource, int count)
        {
            if (waveSource == null)
                throw new ArgumentNullException(nameof(waveSource));
            count -= (count % waveSource.WaveFormat.BlockAlign);
            if(count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var buffer = new byte[count];
            var read = waveSource.Read(buffer, 0, buffer.Length);
            if(read < count)
                Array.Resize(ref buffer, read);
            return buffer;
        }

        internal static bool IsClosed(this Stream stream)
        {
            return !stream.CanRead && !stream.CanWrite;
        }
    }
}
