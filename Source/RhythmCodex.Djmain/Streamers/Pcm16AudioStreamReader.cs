using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Djmain.Streamers
{
    public class Pcm16AudioStreamReader : IPcm16AudioStreamReader
    {
        private const long EndOfStream = 0x4000400040004000 << 1;

        public IList<byte> Read(Stream stream)
        {
            return DecodeStream(stream).ToArray();
        }

        private static IEnumerable<byte> DecodeStream(Stream stream)
        {
            var buffer0 = EndOfStream;
            var buffer1 = EndOfStream;

            void Fetch()
            {
                buffer0 = ((buffer0 >> 16) & 0xFFFFFFFFFFFF) | (buffer1 << 48);
                buffer1 = (buffer1 >> 16) & 0xFFFFFFFFFFFF;

                var newByte = stream.ReadByte();
                if (newByte == -1)
                    newByte = 0x00;
                buffer1 |= (long)newByte << 48;

                newByte = stream.ReadByte();
                if (newByte == -1)
                    newByte = 0x80;
                buffer1 |= (long)newByte << 56;
            }

            // Preload buffer.
            for (var i = 0; i < 8; i++)
                Fetch();

            // Load sample.
            while (buffer0 != EndOfStream || buffer1 != EndOfStream)
            {
                yield return unchecked((byte)buffer0);
                yield return unchecked((byte)(buffer0 >> 8));
                Fetch();
            }
        }
    }
}
