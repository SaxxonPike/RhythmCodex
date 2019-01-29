using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Iso.Model;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.Xa.Heuristics
{
    [Service]
    public class XaIsoStreamFinder : IXaIsoStreamFinder
    {
        public IList<XaChunk> Find(IEnumerable<Iso9660SectorInfo> sectors)
        {
            var result = new List<XaChunk>();
            var mode2Sectors = sectors
                .Where(s => s.Mode == 2)
                .OrderBy(s => (s.Minutes << 16) | (s.Seconds << 8) | s.Frames)
                .ToList();
            var currentStreams = mode2Sectors
                .Where(s => s.Channel != null)
                .GroupBy(s => s.Channel)
                .ToDictionary(g => g.Key, g => new List<Iso9660SectorInfo>());
            var streamCount = currentStreams.Max(s => s.Key).Value + 1;
            var currentStream = 0;

            void AddCurrentStream()
            {
                if (currentStreams[currentStream].Count >= 2)
                {
                    result.Add(new XaChunk
                    {
                        Channels = currentStreams[currentStream].First().AudioChannels.Value,
                        Data = currentStreams[currentStream].SelectMany(s => s.Data.Slice(s.UserDataOffset, s.UserDataLength)).ToArray(),
                        Rate = currentStreams[currentStream].First().AudioRate.Value
                    });                    
                }
                
                currentStreams[currentStream].Clear();
            }
            
            foreach (var sector in mode2Sectors)
            {
                if (sector.IsAudio ?? false)
                    currentStreams[currentStream].Add(sector);
                else
                    AddCurrentStream();

                currentStream = (currentStream + 1) % streamCount;
            }

            for (currentStream = 0; currentStream < streamCount; currentStream++)
                AddCurrentStream();

            return result;
        }
    }
}