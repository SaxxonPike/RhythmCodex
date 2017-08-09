using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Djmain.Model;

namespace RhythmCodex.Djmain.Streamers
{
    public class ChunkStreamReader : IChunkStreamReader
    {
        public IEnumerable<DjmainChunk> Read(Stream stream)
        {
            var length = DjmainConstants.ChunkSize;
            var buffer = new byte[length];
            var id = 0;

            while (true)
            {
                var offset = 0;
                var output = new byte[length];
                var outId = id++;

                while (offset < length)
                {
                    var bytesRead = stream.Read(buffer, offset, length - offset);
                    if (bytesRead == 0)
                        yield break;

                    offset += bytesRead;
                }

                Array.Copy(buffer, output, length);

                yield return new DjmainChunk
                {
                    Data = output,
                    Id = outId
                };
            }
        }
    }
}
