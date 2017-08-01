using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Djmain.Streamers
{
    public class Pcm8AudioStreamReader : IPcm8AudioStreamReader
    {
        private const long EndOfStream = 0x4040404040404040 << 1;

        public IList<byte> Read(Stream stream)
        {
            return DecodeStream(stream).ToArray();
        }

        private static IEnumerable<byte> DecodeStream(Stream stream)
        {
            var buffer = EndOfStream;

            void Fetch()
            {
                long newByte = stream.ReadByte();
                if (newByte < 0)
                    newByte = 0x80;
                buffer = ((buffer >> 8) & 0xFFFFFFFFFFFFFF) | (newByte << 56);
            }

            // Preload buffer.
            for (var i = 0; i < 8; i++)
                Fetch();

            // Load sample.
            while (buffer != EndOfStream)
            {
                yield return unchecked((byte)buffer);
                Fetch();
            }
        }
    }
}
