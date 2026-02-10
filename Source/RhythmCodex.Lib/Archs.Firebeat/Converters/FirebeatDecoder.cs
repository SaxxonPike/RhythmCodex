using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Archs.Firebeat.Models;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Archs.Firebeat.Converters;

[Service]
public class FirebeatDecoder(
    IFirebeatChartDecoder chartDecoder,
    IFirebeatSoundDecoder soundDecoder,
    IFirebeatSampleDecoder sampleDecoder)
    : IFirebeatDecoder
{
    private static readonly List<int> BeatmaniaChartOffsets =
    [
        0x802000,
        0x807000,
        0x80C000,
        0x811000,
        0x816000,
        0x81B000,
        0x820000,
        0x825000,
        0x82A000,
        0x82F000,
        0x834000,
        0x839000
    ];

    private const int BeatmaniaSampleInfoOffset = 0x800000;
    private const int BeatmaniaBgmOffset = 0x0840000;
    private const int TrashSectorSize = 0x200;
    private const int TrashSectorOffset = BeatmaniaBgmOffset - TrashSectorSize;

    public FirebeatArchive? Decode(FirebeatChunk chunk, FirebeatDecodeOptions options)
    {
        var charts = ExtractBeatmaniaCharts(chunk.Data.Span);

        if (charts.Count < 1)
        {
            return null;
        }

        var sampleInfos = ExtractBeatmaniaSampleInfos(chunk.Data.Span);

        var outCharts = charts
            .Select(c =>
            {
                var decoded = chartDecoder.Decode(c.Events);
                decoded[NumericData.Id] = c.Id;
                decoded[NumericData.ByteOffset] = c.Offset;
                return decoded;
            })
            .ToList();

        var outSamples = sampleDecoder
            .Decode(chunk.Data.Span, sampleInfos);

        var outSounds = soundDecoder
            .Decode(outSamples);

        return new FirebeatArchive
        {
            Id = chunk.Id,
            Charts = outCharts,
            RawCharts = charts.ToDictionary(c => c.Id, c => c.Events),
            RawSampleInfos = sampleInfos,
            Samples = outSounds.Values.ToList()
        };
    }

    private static FirebeatSampleInfo? GetBeatmaniaBgmInfo(ReadOnlySpan<byte> data)
    {
        for (var j = data.Length - 4; j >= BeatmaniaBgmOffset + 4; j -= 4)
        {
            if (data[j] == 0x00 &&
                data[j + 1] == 0x80 &&
                data[j + 2] == 0x00 &&
                data[j + 3] == 0x80)
                continue;

            return new FirebeatSampleInfo
            {
                Channel = 0xFF,
                Flag01 = 0x00,
                Frequency = 0xAC44,
                Volume = 0x01,
                Panning = 0x40,
                SampleOffset = BeatmaniaBgmOffset / 2,
                SampleLength = (j - BeatmaniaBgmOffset + 4) / 2,
                Value0C = 0x0000,
                Flag0E = 0x07,
                Flag0F = FirebeatSampleFlag0F.Stereo,
                SizeInBlocks = 0
            };
        }

        return null;
    }

    private static Dictionary<int, FirebeatSampleInfo> ExtractBeatmaniaSampleInfos(ReadOnlySpan<byte> data)
    {
        var offset = BeatmaniaSampleInfoOffset;
        var sampleInfos = new Dictionary<int, FirebeatSampleInfo>();

        //
        // Load sample info table.
        //

        for (var sampleId = 0; sampleId < 256; sampleId++)
        {
            var info = data[offset..];

            if (info.Length < 0x12)
                break;

            if (info[0x01..0x12].IndexOfAnyExcept(info[0x00]) < 0)
                continue;

            var channel = info[0x00];
            var flag01 = info[0x01];
            var frequency = ReadUInt16BigEndian(info[0x02..]);
            var volume = info[0x04];
            var panning = info[0x05];
            var sampleStart = (info[0x06] << 16) | ReadUInt16BigEndian(info[0x07..]);
            var sampleStop = (info[0x09] << 16) | ReadUInt16BigEndian(info[0x0A..]);
            var value0C = ReadUInt16BigEndian(info[0x0C..]);
            var flag0E = info[0x0E];
            var flag0F = info[0x0F];
            var sizeInBlocks = ReadUInt16BigEndian(info[0x10..]);

            sampleInfos[sampleId + 1] = new FirebeatSampleInfo
            {
                Channel = channel,
                Flag01 = flag01,
                Frequency = frequency,
                Volume = volume,
                Panning = panning,
                SampleLength = sampleStop - sampleStart,
                SampleOffset = sampleStart,
                Value0C = value0C,
                Flag0E = flag0E,
                Flag0F = (FirebeatSampleFlag0F)flag0F,
                SizeInBlocks = sizeInBlocks
            };

            offset += 0x12;
        }

        if (GetBeatmaniaBgmInfo(data) is { } bgmInfo)
            sampleInfos[0] = bgmInfo;

        return sampleInfos;
    }

    private static List<FirebeatChart> ExtractBeatmaniaCharts(ReadOnlySpan<byte> data)
    {
        var result = new List<FirebeatChart>();
        var id = -1;

        foreach (var chartOffset in BeatmaniaChartOffsets)
        {
            ++id;

            var offset = chartOffset;

            //
            // If the first 32 bytes of a chart offset are the same byte, it is unlikely
            // that the data really is a chart.
            //

            if (data.Slice(offset + 1, 0x1F).IndexOfAnyExcept(data[offset]) < 0)
                continue;

            //
            // One extra safety check against known *bad* repeating data.
            //

            if (data.Slice(TrashSectorOffset, TrashSectorSize).SequenceEqual(data.Slice(offset, TrashSectorSize)))
                continue;

            //
            // Read the header.
            //

            var header = data.Slice(offset, 0x20);
            offset += 0x20;

            //
            // The first two bytes of event data will always be zero, followed by a non-zero byte.
            //

            var firstEvent = data.Slice(offset, 6);

            if (firstEvent[0] != 0 ||
                firstEvent[1] != 0 ||
                firstEvent[2] == 0)
                continue;
            
            if (header[0x1C] != 0x00 ||
                header[0x1D] != 0x00 ||
                header[0x1E] != 0x00 ||
                header[0x1F] != 0x00)
                continue;

            //
            // Read each event.
            //

            var tick = 0;
            var events = new List<FirebeatChartEvent>();

            while (true)
            {
                var ev = data[offset..];

                if (ev.Length < 6)
                    break;

                var delta = ReadUInt16BigEndian(ev);
                var type = ev[2];
                var player = ev[3];
                var value = ReadUInt16BigEndian(ev[4..]);

                tick += delta;

                events.Add(new FirebeatChartEvent
                {
                    Tick = tick,
                    Type = type,
                    Player = player,
                    Data = value
                });

                if (type == 0xFF)
                    break;

                offset += 6;
            }

            //
            // Keep only the charts with any actual events.
            //

            if (events.Count > 0)
            {
                result.Add(new FirebeatChart
                {
                    Id = id,
                    Offset = chartOffset,
                    Header = header.ToArray(),
                    Events = events
                });
            }
        }

        return result;
    }
}