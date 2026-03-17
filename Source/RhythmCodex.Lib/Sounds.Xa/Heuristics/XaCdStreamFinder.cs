using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Xa.Models;

namespace RhythmCodex.Sounds.Xa.Heuristics;

[Service]
public class XaCdStreamFinder : IXaCdStreamFinder
{
    public IEnumerable<XaChunk> FindMode2(IEnumerable<IsoSectorInfo> sectors)
    {
        var mode2Sectors = sectors
            .Where(s => s.Mode == 2)
            .ToList();

        var currentStreams = new Dictionary<int, List<IsoSectorInfo>>();

        foreach (var sector in mode2Sectors)
        {
            var channel = sector.Channel!.Value;

            if (!currentStreams.TryGetValue(channel, out var currentStream))
            {
                currentStream = [];
                currentStreams[channel] = currentStream;
            }

            if (sector.IsAudio ?? false)
                currentStream.Add(sector);
        }

        foreach (var (_, val) in currentStreams)
            if (CreateChunk(val) is { } chunk)
                yield return chunk;

        yield break;

        XaChunk? CreateChunk(List<IsoSectorInfo> sectorInfos)
        {
            XaChunk? chunk = null;

            if (sectorInfos.Count >= 2)
            {
                chunk = new XaChunk
                {
                    Channels = sectorInfos[0].AudioChannels!.Value,
                    Data = sectorInfos.Select(s => s.UserData).Combine(),
                    Rate = sectorInfos[0].AudioRate!.Value
                };
            }

            sectorInfos.Clear();
            return chunk;
        }
    }
}