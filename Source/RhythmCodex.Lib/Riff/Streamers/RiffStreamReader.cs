using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Riff.Models;

namespace RhythmCodex.Riff.Streamers;

[Service]
public class RiffStreamReader : IRiffStreamReader
{
    private readonly IRiffChunkStreamReader _chunkStreamReader;

    public RiffStreamReader(IRiffChunkStreamReader chunkStreamReader)
    {
        _chunkStreamReader = chunkStreamReader;
    }
        
    public IRiffContainer Read(Stream stream)
    {
        var reader = new BinaryReader(stream, Encodings.CP437);
        var id = new string(reader.ReadChars(4));
        if (id != "RIFF")
            throw new RhythmCodexException("Missing RIFF header.");
        var length = reader.ReadInt32() - 4;
        var format = new string(reader.ReadChars(4));
        var chunks = new List<IRiffChunk>();

        while (length > 0)
        {
            var chunk = _chunkStreamReader.Read(stream);
            length -= chunk.Data.Length + 8;
            chunks.Add(chunk);
        }

        return new RiffContainer
        {
            Chunks = chunks,
            Format = format
        };
    }
}