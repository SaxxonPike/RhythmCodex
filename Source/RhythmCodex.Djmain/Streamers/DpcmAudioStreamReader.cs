using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RhythmCodex.Djmain.Streamers
{
    public class DpcmAudioStreamReader : IDpcmAudioStreamReader
    {
        private const int EndOfStream = 0x44444444 << 1;
        
        public IList<byte> Read(Stream stream)
        {
            return ReadStream(stream).ToArray();
        }

        private static IEnumerable<byte> ReadStream(Stream stream)
        {
            var buffer = EndOfStream;

            void Fetch()
            {
                buffer >>= 8;
                var newByte = stream.ReadByte();
                if (newByte < 0)
                    newByte = 0x88;
                buffer = (buffer & 0xFFFFFF) | (newByte << 24);
            }

            for (var i = 0; i < 4; i++)
                Fetch();

            while (buffer != EndOfStream)
            {
                yield return unchecked((byte) buffer);
                Fetch();
            }
        }
    }
}
