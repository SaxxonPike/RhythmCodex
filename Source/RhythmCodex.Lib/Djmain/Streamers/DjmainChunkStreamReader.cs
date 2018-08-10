using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Streamers
{
    [Service]
    public class DjmainChunkStreamReader : IDjmainChunkStreamReader
    {
        public IEnumerable<DjmainChunk> Read(Stream stream)
        {
            const int length = DjmainConstants.ChunkSize;
            var buffer = new byte[length];
            var id = 0;
            DjmainChunkFormat? format = null;

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

                if (format == null)
                    format = DetectFormat(output);

                yield return new DjmainChunk
                {
                    Format = format.Value,
                    Data = output,
                    Id = outId
                };
            }
        }

        private DjmainChunkFormat DetectFormat(byte[] output)
        {
            var formatId = Encoding.ASCII.GetString(new[]
            {
                output[1], output[0], output[3], output[2], output[5]
            });
            
            switch (formatId)
            {
                case @"GQ753": return DjmainChunkFormat.First;
                case @"GX853": return DjmainChunkFormat.Second;
                case @"GQ825": return DjmainChunkFormat.Third;
                case @"GQ847": return DjmainChunkFormat.Fourth;
                case @"GQ981": return DjmainChunkFormat.Fifth;
                case @"GCA21": return DjmainChunkFormat.Sixth;
                case @"GEB07": return DjmainChunkFormat.Seventh;
                case @"GQ993": return DjmainChunkFormat.Club;
                case @"GQ858": return DjmainChunkFormat.Complete;
                case @"GQ988": return DjmainChunkFormat.Complete2;
                case @"GQA05": return DjmainChunkFormat.Core;
                case @"GQ995": return DjmainChunkFormat.Dct;
                case @"GCC01": return DjmainChunkFormat.Final;
                default: return DjmainChunkFormat.Unknown;
            }
        }
    }
}