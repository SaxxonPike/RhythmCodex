using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.FileSystems.Iso.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Xa.Models;

namespace RhythmCodex.Sounds.Xa.Heuristics;

[Service]
public class XaCdStreamFinder : IXaCdStreamFinder
{
    public IEnumerable<XaChunk> FindMode2(IEnumerable<IsoSectorInfo> sectors, bool splitByEmpty)
    {
        //
        // This determines how many consecutive empty sectors are permitted before a split.
        // Lower values for "empty sector threshold" are less likely to combine
        // BGMs unintentionally, but are more likely to split BGMs that have silent parts.
        //

        const int emptySectorThreshold = 16;

        //
        // This determines the minimum number of sectors that is considered for a valid
        // chunk. Consecutive runs of sectors that are less than this size are skipped.
        //

        const int minimumSectorsThreshold = 8;

        //
        // Create the empty sector for comparison.
        // This should match the user data block for a mode 2 form 2 sector.
        //

        var emptySector = new byte[2304];

        for (var i = 0; i < emptySector.Length; i += 128)
            emptySector.AsSpan(i, 16).Fill(0x0C);

        //
        // These hold export state for each channel.
        //

        var emptySectorCounts = new Dictionary<int, int>();
        var sourceIndices = new Dictionary<int, int>();
        var activeChannels = new HashSet<int>();

        //
        // Collect mode 2, form 2 sectors.
        //

        var mode2Sectors = sectors
            .Where(s => s is { Mode: 2, Form: 2 });

        var currentStreams = new Dictionary<int, List<IsoSectorInfo>>();

        //
        // Split the sectors up into their assigned channels.
        //

        foreach (var sector in mode2Sectors)
        {
            if (sector.IsAudio != true)
                continue;

            var channel = sector.Channel!.Value;

            if (!currentStreams.TryGetValue(channel, out var currentStream))
            {
                currentStream = [];
                currentStreams[channel] = currentStream;
                emptySectorCounts[channel] = emptySectorThreshold;
                sourceIndices[channel] = 0;
            }

            var emptySectorCount = emptySectorCounts[channel];

            var isEmpty = sector.UserData.Span.SequenceEqual(emptySector.AsSpan());

            if (isEmpty)
            {
                emptySectorCounts[channel]++;

                if (splitByEmpty && emptySectorCount >= emptySectorThreshold && activeChannels.Remove(channel))
                {
                    if (CreateChunk(currentStream, channel) is { } chunk)
                        yield return chunk;
                }
            }
            else
            {
                activeChannels.Add(channel);
                emptySectorCounts[channel] = 0;
            }

            if (activeChannels.Contains(channel))
                currentStream.Add(sector);
        }

        //
        // Flush buffers for sectors that run into the end of the data.
        //

        foreach (var (channel, val) in currentStreams)
            if (CreateChunk(val, channel) is { } chunk)
                yield return chunk;

        yield break;

        //
        // This function builds a chunk and adds it to the result.
        //

        XaChunk? CreateChunk(List<IsoSectorInfo> sectorInfos, int ch)
        {
            XaChunk? chunk = null;

            if (sectorInfos.Count >= minimumSectorsThreshold)
            {
                chunk = new XaChunk
                {
                    SourceSector = sectorInfos.Min(s => s.Number),
                    SourceChannel = ch,
                    SourceIndex = sourceIndices[ch],
                    Channels = sectorInfos[0].AudioChannels!.Value,
                    Data = sectorInfos.Select(s => s.UserData).Combine(),
                    Rate = sectorInfos[0].AudioRate!.Value
                };

                sourceIndices[ch] += 1;
            }

            sectorInfos.Clear();
            return chunk;
        }
    }
}