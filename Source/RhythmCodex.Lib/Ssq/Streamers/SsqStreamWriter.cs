﻿using System.Collections.Generic;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Streamers
{
    [Service]
    public class SsqStreamWriter : ISsqStreamWriter
    {
        private readonly IChunkStreamWriter _chunkStreamWriter;

        public SsqStreamWriter(IChunkStreamWriter chunkStreamWriter)
        {
            _chunkStreamWriter = chunkStreamWriter;
        }

        public void Write(Stream stream, IEnumerable<SsqChunk> chunks)
        {
            foreach (var chunk in chunks)
                _chunkStreamWriter.Write(stream, chunk);

            _chunkStreamWriter.Write(stream, null);
        }
    }
}