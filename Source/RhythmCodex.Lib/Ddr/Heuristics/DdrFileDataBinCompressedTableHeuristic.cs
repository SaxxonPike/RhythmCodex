using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Heuristics;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Heuristics
{
    [Service]
    [Context(Context.DdrCs)]
    public class DdrFileDataBinCompressedTableHeuristic : IReadableHeuristic<IList<DdrPs2FileDataTableEntry>>
    {
        private readonly IDdrPs2FileDataTableChunkStreamReader _ddrPs2FileDataTableChunkStreamReader;
        private readonly IDdrPs2FileDataUnboundTableDecoder _ddrPs2FileDataUnboundTableDecoder;

        public DdrFileDataBinCompressedTableHeuristic(
            IDdrPs2FileDataTableChunkStreamReader ddrPs2FileDataTableChunkStreamReader,
            IDdrPs2FileDataUnboundTableDecoder ddrPs2FileDataUnboundTableDecoder)
        {
            _ddrPs2FileDataTableChunkStreamReader = ddrPs2FileDataTableChunkStreamReader;
            _ddrPs2FileDataUnboundTableDecoder = ddrPs2FileDataUnboundTableDecoder;
        }

        public IList<DdrPs2FileDataTableEntry> Read(HeuristicResult heuristicResult, Stream stream)
        {
            var table = _ddrPs2FileDataTableChunkStreamReader.GetUnbound(stream);
            var decoded = _ddrPs2FileDataUnboundTableDecoder.Decode(table);
            return decoded;
        }

        public string Description => "FILEDATA.BIN compressed data table";

        public string FileExtension => "fdbtable";
        
        public HeuristicResult Match(IHeuristicReader reader)
        {
            var maxTable = int.MaxValue;
            var offsets = new List<int>();

            for (var i = 0; i < maxTable; i++)
            {
                var offset = reader.ReadInt();
                if (offset >= 4 && offset < 0x1000000 && !offsets.Contains(offset))
                {
                    offsets.Add(offset);
                    if (maxTable > offset / 4)
                        maxTable = offset / 4;
                }
                else if (offset != 0 && offset < i * 4)
                {
                    return null; 
                }
            }
            
            return new HeuristicResult(this);
        }
    }
}