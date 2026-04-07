using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Converters;

[Service]
public class IsoDescriptorSectorFinder : IIsoDescriptorSectorFinder
{
    public List<IsoSectorInfo> Find(IEnumerable<IsoSectorInfo> sectors)
    {
        return FindInternal(sectors).ToList();
    }
        
    private static List<IsoSectorInfo> FindInternal(IEnumerable<IsoSectorInfo> sectors)
    {
        var result = new List<IsoSectorInfo>();

        var sectorEnumerator = sectors
            .Where(s => s.Mode == 1 || s is { Mode: 2, Form: 1 })
            .GetEnumerator();

        var foundDescriptor = false;
        var currentMinute = 0;
        var currentSecond = 0;
        var currentFrame = 16;

        while (sectorEnumerator.MoveNext())
        {
            var sector = sectorEnumerator.Current;

            if (foundDescriptor && (sector.Frames != currentFrame ||
                                    sector.Seconds != currentSecond ||
                                    sector.Minutes != currentMinute))
                continue;

            if (sector == null)
                throw new RhythmCodexException($"Volume descriptors incomplete. MSF={currentMinute:000}:{currentSecond:00}:{currentFrame:00}");

            var userData = sector.UserData.Span;

            if (!userData.Slice(0x0001, 5).SequenceEqual("CD001"u8))
            {
                continue;
            }

            foundDescriptor = true;
            result.Add(sector);

            currentFrame = sector.Frames ?? 0;
            currentSecond = sector.Seconds ?? 0;
            currentMinute = sector.Minutes ?? 0;
                
            if (userData[0x0000] == 255)
                break;

            currentFrame++;
                
            if (currentFrame >= 75)
            {
                currentFrame = 0;
                currentSecond++;
            }

            if (currentSecond >= 60)
            {
                currentSecond = 0;
                currentMinute++;
            }
        }

        return result;
    }
}