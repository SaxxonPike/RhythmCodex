using System.Collections.Generic;
using System.Linq;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Xa.Models;

namespace RhythmCodex.Sounds.Xa.Heuristics;

[Service]
public class XaIsoStreamFinder : IXaIsoStreamFinder
{
    public List<XaChunk> Find(IEnumerable<IsoSectorInfo> sectors)
    {
        var result = new List<XaChunk>();
        var mode2Sectors = sectors
            .Where(s => s.Mode == 2)
            .OrderBy(s => (s.Minutes << 16) | (s.Seconds << 8) | s.Frames)
            .ToList();
        var currentStreams = mode2Sectors
            .Where(s => s.Channel != null)
            .GroupBy(s => s.Channel)
            .ToDictionary(g => (int)g.Key!, _ => new List<IsoSectorInfo>());
        var streamCount = currentStreams.Max(s => s.Key) + 1;
        var currentStream = 0;

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

        void AddCurrentStream()
        {
            if (currentStreams[currentStream].Count >= 2)
            {
                result.Add(new XaChunk
                {
                    Channels = currentStreams[currentStream].First().AudioChannels!.Value,
                    Data = currentStreams[currentStream].Select(s =>
                        s.Data.Slice(s.UserDataOffset, s.UserDataLength)).Combine(),
                    Rate = currentStreams[currentStream].First().AudioRate!.Value
                });
            }

            currentStreams[currentStream].Clear();
        }
    }
}