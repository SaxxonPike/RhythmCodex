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
    }
}