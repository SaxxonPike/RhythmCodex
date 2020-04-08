using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Step1.Models;
using RhythmCodex.Step1.Streamers;

namespace RhythmCodex.Step1.Heuristics
{
    [Service]
    public class Step1Heuristic : IReadableHeuristic<IEnumerable<Step1Chunk>>
    {
        private readonly IStep1StreamReader _step1StreamReader;

        public Step1Heuristic(IStep1StreamReader step1StreamReader)
        {
            _step1StreamReader = step1StreamReader;
        }
        
        public string Description => "DDR Step Sequence (older)";
        public string FileExtension => "step";

        public HeuristicResult Match(IHeuristicReader reader)
        {
            // Must have at least 4 bytes
            if (reader.Length == null || reader.Length < 4)
                return null;
            
            // Must be divisible by 4
            if ((reader.Length & 0x3) != 0)
                return null;

            var data = reader.Read((int) reader.Length);
            
            // Make sure each chunk length makes sense
            var thisOffset = 0;
            while (thisOffset < reader.Length)
            {
                var thisChunkLength = Bitter.ToInt32(data, thisOffset);
                if (thisChunkLength == 0)
                    break;
                if (thisChunkLength < 0)
                    return null;
                if (thisOffset + thisChunkLength >= reader.Length)
                    return null;
                if ((thisOffset & 0x3) != 0)
                    return null;
                thisOffset += thisChunkLength;
            }
            
            // Try to make sense of the timing chunk length
            var timingChunkLength = Bitter.ToInt32(data);
            if (timingChunkLength < 20)
                return null;
            if (timingChunkLength >= reader.Length)
                return null;
            if ((timingChunkLength & 0x3) != 0)
                return null;
            
            // Check both the measure and the second offset, make sure they are increasing
            var timingMeasure = int.MinValue;
            var timingSector = int.MinValue;
            for (var i = 1; i < timingChunkLength >> 2; i += 2)
            {
                var thisTimingMeasure = Bitter.ToInt32(data, i << 2);
                var thisTimingSector = Bitter.ToInt32(data, 0x4 + (i << 2));
                if (thisTimingMeasure < timingMeasure)
                    return null;
                if (thisTimingSector < timingSector)
                    return null;
                timingMeasure = thisTimingMeasure;
                timingSector = thisTimingSector;
            }
            
            // No reason to believe this isn't a step1 at this point
            return new HeuristicResult(this);
        }

        public IEnumerable<Step1Chunk> Read(HeuristicResult heuristicResult, Stream stream)
        {
            return _step1StreamReader.Read(stream);
        }
    }
}