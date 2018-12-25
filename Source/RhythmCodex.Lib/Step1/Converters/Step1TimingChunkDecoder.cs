using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Step1.Models;

namespace RhythmCodex.Step1.Converters
{
    [Service]
    public class Step1TimingChunkDecoder : IStep1TimingChunkDecoder
    {
        public TimingChunk Convert(byte[] data)
        {
            return new TimingChunk
            {
                Timings = ConvertInternal(data).ToList(), 
                Rate = 75
            };
        }

        private IEnumerable<Timing> ConvertInternal(byte[] data)
        {
            using (var mem = new ReadOnlyMemoryStream(data))
            using (var reader = new BinaryReader(mem))
            {
                while (mem.Position < mem.Length - 7)
                {
                    yield return new Timing
                    {
                        MetricOffset = reader.ReadInt32(),
                        LinearOffset = reader.ReadInt32()
                    };
                }
            }
        }
    }
}