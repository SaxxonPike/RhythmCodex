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
    public class DdrFileDataBinCompressedTableHeuristic : IReadableHeuristic<IList<DdrPs2FileDataTableEntry>>
    {
        private readonly IDdrPs2FileDataTableStreamReader _ddrPs2FileDataTableStreamReader;
        private readonly IDdrPs2FileDataTableDecoder _ddrPs2FileDataTableDecoder;

        public DdrFileDataBinCompressedTableHeuristic(
            IDdrPs2FileDataTableStreamReader ddrPs2FileDataTableStreamReader,
            IDdrPs2FileDataTableDecoder ddrPs2FileDataTableDecoder)
        {
            _ddrPs2FileDataTableStreamReader = ddrPs2FileDataTableStreamReader;
            _ddrPs2FileDataTableDecoder = ddrPs2FileDataTableDecoder;
        }

        public IList<DdrPs2FileDataTableEntry> Read(HeuristicResult heuristicResult, Stream stream)
        {
            var table = _ddrPs2FileDataTableStreamReader.Get(stream);
            var decoded = _ddrPs2FileDataTableDecoder.Decode(table);
            return decoded;
        }

        public string Description => "FILEDATA.BIN compressed data table";

        public string FileExtension => "fdbtable";
        
        public HeuristicResult Match(ReadOnlySpan<byte> data)
        {
            var maxTable = data.Length / 4;
            var offsetBlock = MemoryMarshal.Cast<byte, int>(data);
            var offsets = new List<int>();

            for (var i = 0; i < maxTable; i++)
            {
                var offset = offsetBlock[i];
                if (offset >= 4 && offset < 0x1000000 && !offsets.Contains(offset))
                {
                    offsets.Add(offset);
                }
                else
                {
                    return null; 
                }

                if (maxTable > offset / 4)
                    maxTable = offset / 4;
            }
            
            return new HeuristicResult(this);
        }

        public HeuristicResult Match(Stream stream)
        {
            throw new NotImplementedException();
        }

        public int MinimumLength => 5;
    }
}