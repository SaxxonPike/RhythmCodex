using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Djmain.Heuristics;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Djmain.Streamers;

[Service]
public class DjmainChunkStreamReader(IDjmainHddDescriptionHeuristic djmainHddDescriptionHeuristic)
    : IDjmainChunkStreamReader
{
    public IEnumerable<DjmainChunk> Read(Stream stream)
    {
        const int length = DjmainConstants.ChunkSize;
        var id = 0;
        DjmainHddDescription? format = null;

        while (true)
        {
            var offset = 0;
            var output = new byte[length];
            var outId = id++;

            while (offset < length)
            {
                var bytesRead = stream.Read(output.AsSpan(offset));
                if (bytesRead == 0)
                    yield break;

                offset += bytesRead;
            }

            format ??= djmainHddDescriptionHeuristic.Get(output);

            if (format.BytesAreSwapped)
                output.AsSpan().Swap16();

            yield return new DjmainChunk
            {
                Format = format.Format,
                Data = output,
                Id = outId
            };
        }
    }
}