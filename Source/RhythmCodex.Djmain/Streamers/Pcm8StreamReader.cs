using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Djmain.Streamers
{
    public class Pcm8StreamReader : IPcm8StreamReader
    {
        public IList<byte> Read(Stream stream)
        {
            return DecodeStream(stream).ToArray();
        }

        private static IEnumerable<byte> DecodeStream(Stream stream)
        {
            var marker = DjmainConstants.Pcm8EndMarker;
            var buffer = marker;

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
            while (buffer != marker)
            {
                yield return unchecked((byte)buffer);
                Fetch();
            }
        }
    }
}
