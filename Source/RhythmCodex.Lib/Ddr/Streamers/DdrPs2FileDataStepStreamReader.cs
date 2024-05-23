using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq;

namespace RhythmCodex.Ddr.Streamers;

[Service]
public class DdrPs2FileDataStepStreamReader : IDdrPs2FileDataStepStreamReader
{
    private readonly IBemaniLzDecoder _bemaniLzDecoder;
    private readonly IHeuristicTester _heuristicTester;

    public DdrPs2FileDataStepStreamReader(IBemaniLzDecoder bemaniLzDecoder, IHeuristicTester heuristicTester)
    {
        _bemaniLzDecoder = bemaniLzDecoder;
        _heuristicTester = heuristicTester;
    }
        
    public DdrPs2FileDataTableChunk Read(Stream fileDataBinStream, long length)
    {
        var position = -0x800L;
        var max = length / 0x800 * 0x800;
        var buffer = new MemoryStream();
        var reader = new BinaryReader(fileDataBinStream);
        var skip = false;
        var append = false;
        Span<byte> empty = stackalloc byte[16];
        var success = false;
        var offsets = new List<int>();

        while (position < max)
        {
            position += 0x800;
            // if (position == 0x433E1000)
            //     position = position;
                
            var block = reader.ReadBytes(0x800);

            if (append)
                buffer.Write(block, 0, block.Length);
                
            if (skip)
            {
                skip = !block.AsSpan(0x7F0).SequenceEqual(empty);
                append &= skip;
                if (!skip && success)
                {
                    var oldPos = buffer.Position;
                    buffer.Position = offsets[0];
                    try
                    {
                        var lzBuffer = _bemaniLzDecoder.Decode(buffer);
                        var matches = _heuristicTester.Match(lzBuffer).Where(t => t.Heuristic is SsqHeuristic);
                        if (!matches.Any())
                            success = false;
                    }
                    catch (Exception)
                    {
                        success = false;
                    }
                    finally
                    {
                        buffer.Position = oldPos;
                    }

                    if (!success)
                    {
                        buffer.Position = 0;
                        buffer.SetLength(0);
                        continue;
                    }

                    return new DdrPs2FileDataTableChunk
                    {
                        Data = buffer.ToArray(),
                        HasHeaders = false
                    };
                }

                continue;
            }

            append = true;
            buffer.Write(block, 0, block.Length);

            var maxTable = block.Length / 4;
            success = true;
            var offsetBlock = Bitter.ToInt32Values(block);
            offsets.Clear();

            for (var i = 0; i < maxTable; i++)
            {
                var offset = offsetBlock[i];
                if (offset is >= 4 and < 0x1000000 && !offsets.Contains(offset))
                {
                    offsets.Add(offset);
                }
                else
                {
                    success = false; 
                    break;
                }
                if (maxTable > offset / 4)
                    maxTable = offset / 4;
            }

            skip = true;

            if (success)
                continue;
                
            buffer.Position = 0;
            buffer.SetLength(0);
            append = false;
        }

        return null;
    }
}