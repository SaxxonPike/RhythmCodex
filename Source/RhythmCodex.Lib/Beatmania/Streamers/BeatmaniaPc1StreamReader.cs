using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Streamers
{
    [Service]
    public class BeatmaniaPc1StreamReader : IBeatmaniaPc1StreamReader
    {
        public IEnumerable<BeatmaniaPc1Chart> Read(Stream source, long length)
        {
            var buffer = new byte[length];
            source.TryRead(buffer, 0, (int) length);
            var reader = new BinaryReader(new ReadOnlyMemoryStream(buffer));
            
            var offsets = Enumerable.Range(0, 12)
                .ToDictionary(i => i, _ =>
                {
                    var result = reader.ReadInt32();
                    reader.ReadInt32(); // length
                    return result;
                });

            foreach (var offset in offsets.Where(kv => kv.Value >= 0x60).OrderBy(kv => kv.Value))
            {
                reader.BaseStream.Position = offset.Value;
                var data = new List<BeatmaniaPc1Event>();

                while (true)
                {
                    var linearOffset = reader.ReadInt32();
                    if (linearOffset == 0x7FFFFFFF)
                        break;
                    
                    var ev = new BeatmaniaPc1Event
                    {
                        LinearOffset = linearOffset,
                        Parameter0 = reader.ReadByte(),
                        Parameter1 = reader.ReadByte(),
                        Value = reader.ReadInt16()
                    };
                    
                    data.Add(ev);

                }
                
                yield return new BeatmaniaPc1Chart
                {
                    Index = offset.Key,
                    Data = data
                };
            }
        }
    }
}