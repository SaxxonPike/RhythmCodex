using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Cli.Helpers
{
    public static class StreamExtensions
    {
        private const int BufferSize = 4096;

        public static void WriteAllBytes(this byte[] data, Stream stream)
        {
            stream.Write(data, 0, data.Length);
        }
        
        public static byte[] ReadAllBytes(this Stream stream)
        {
            var buffer = new byte[BufferSize];
            var result = new List<byte>();
            while (true)
            {
                var actualBytesRead = stream.Read(buffer, 0, BufferSize);
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
    }
}