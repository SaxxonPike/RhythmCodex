using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Infrastructure
{
    public static class StreamExtensions
    {
        private const int BufferSize = 1 << 16;

        private static byte[] ReadAllBytes(Func<byte[], int, int, int> readFunc)
        {
            var buffer = new byte[BufferSize];
            var result = new List<byte>();
            while (true)
            {
                var actualBytesRead = readFunc(buffer, 0, BufferSize);
                if (actualBytesRead == BufferSize)
                {
                    result.AddRange(buffer);
                }
                else if (actualBytesRead > 0)
                {
                    result.AddRange(buffer.Take(actualBytesRead));
                }
                else
                {
                    break;
                }
            }
            return result.ToArray();
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            return ReadAllBytes(stream.Read);
        }

        public static int TryRead(this Stream stream, byte[] buffer, int offset, int count)
        {
            while (true)
            {
                var actualBytesRead = stream.Read(buffer, offset, count);
                offset += actualBytesRead;
                if (actualBytesRead >= count || actualBytesRead <= 0)
                    break;
                count -= actualBytesRead;
            }

            return offset;
        }

        private static void PipeAllBytes(Func<byte[], int, int, int> read, Action<byte[], int, int> write)
        {
            var buffer = new byte[BufferSize];
            while (true)
            {
                var bytesRead = read(buffer, 0, BufferSize);
                if (bytesRead <= 0)
                    break;
                write(buffer, 0, bytesRead);
            }
        }

        public static void PipeAllBytes(this Stream stream, Stream target)
        {
            PipeAllBytes(stream.Read, target.Write);
        }
        
        public static long Skip(this Stream stream, long length)
        {
            var buffer = new byte[Math.Min(length, 65536)];
            var bufferLength = buffer.Length;
            var result = 0;

            while (length > 0)
            {
                var bytesRead = TryRead(stream, buffer, 0, (int)Math.Min(length, bufferLength));
                if (bytesRead == 0)
                    break;
                length -= bytesRead;
                result += bytesRead;
            }

            return result;
        } 
    }

}