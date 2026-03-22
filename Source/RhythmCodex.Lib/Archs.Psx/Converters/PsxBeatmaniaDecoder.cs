using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Archs.Djmain.Converters;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Archs.Djmain.Streamers;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.Archs.Psx.Streamers;
using RhythmCodex.Charts.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Archs.Psx.Converters;

/// <inheritdoc />
[Service]
public sealed class PsxBeatmaniaDecoder(
    IPsxBeatmaniaFileStreamReader psxBeatmaniaFileStreamReader,
    IDjmainChartEventStreamReader chartEventStreamReader,
    IDjmainChartDecoder djmainChartDecoder
) : IPsxBeatmaniaDecoder
{
    /// <inheritdoc />
    public Chart DecodeChart(Stream source)
    {
        var events = chartEventStreamReader.Read(source);
        var chart = djmainChartDecoder.Decode(events, DjmainChartType.BeatmaniaCs, false);
        return chart;
    }

    /// <inheritdoc />
    public List<PsxBeatmaniaFile> DecodeBmData(Stream source, long length)
    {
        using var memOwner = MemoryPool<byte>.Shared.Rent((int)length);
        var mem = memOwner.Memory[..(int)length];
        source.ReadExactly(mem.Span);
        using var pak = new ReadOnlyMemoryStream(mem);

        Span<byte> temp = stackalloc byte[16];
        var files = new List<PsxBeatmaniaFile>();
        var fileIdx = 0;

        for (var i = 0; i < pak.Length - 0x7FF; i += 0x800)
        {
            pak.Position = i;

            pak.ReadExactly(temp);

            if (ReadInt32LittleEndian(temp) == ReadInt32LittleEndian(temp[8..]) &&
                ReadInt32LittleEndian(temp) is > 0 and < 32768 &&
                ReadInt32LittleEndian(temp[4..]) is > 0 and < 1024 &&
                ReadInt32LittleEndian(temp[8..]) is > 0 and < 32768 &&
                ReadInt32LittleEndian(temp[12..]) > 0)
            {
                try
                {
                    pak.Position = i;
                    var bmDataPakDirectory = psxBeatmaniaFileStreamReader.ReadDirectory(pak);
                    var bmFiles = psxBeatmaniaFileStreamReader.ReadEntries(pak, bmDataPakDirectory);
                    files.AddRange(bmFiles.Select((x, idx) => CreateFile(x) with
                    {
                        Index = fileIdx++,
                        Group = i,
                        GroupIndex = idx
                    }));
                }
                catch
                {
                    // No action taken.
                }
            }
        }

        return files;
    }

    /// <summary>
    /// Returns true if the data is a note chart.
    /// </summary>
    private static bool DetectChart(ReadOnlySpan<byte> span) =>
        span.Length >= 4 &&
        (span.Length & 3) == 0 &&
        ReadInt32LittleEndian(span[^4..]) == 0x00007FFF;

    /// <summary>
    /// Returns true if the data is a keysound block.
    /// </summary>
    private static bool DetectKeysound(ReadOnlySpan<byte> span) =>
        span.Length >= 16 &&
        (span.Length & 7) == 0 &&
        ReadInt32BigEndian(span) < 0x1000 &&
        ReadInt32BigEndian(span) >= 0x0800 &&
        ReadInt32BigEndian(span) != 0 &&
        (ReadInt32BigEndian(span[4..]) & 7) == 0 &&
        ReadInt32BigEndian(span[4..]) != 0 &&
        span[8..16].IndexOfAnyExcept((byte)0x00) < 0;

    /// <summary>
    /// Returns true if the data is a keysound table.
    /// </summary>
    private static bool DetectKst(ReadOnlySpan<byte> span) =>
        span.Length >= 4 &&
        (span.Length & 3) == 0 &&
        ReadInt32BigEndian(span[^4..]) == 0x0000FEFF;

    /// <summary>
    /// Returns true if the data is a DAT3 file.
    /// </summary>
    private static bool DetectDat3(ReadOnlySpan<byte> span) =>
        span.Length >= 8 &&
        (span.Length & 3) == 0 &&
        ReadInt32LittleEndian(span[^8..]) == 0x00000001 &&
        ReadInt32LittleEndian(span[^4..]) == 0x00000000;

    /// <summary>
    /// Determines the type of the data and returns a <see cref="PsxBeatmaniaFile"/>
    /// populated with the detected type.
    /// </summary>
    private static PsxBeatmaniaFile CreateFile(ReadOnlyMemory<byte> data)
    {
        var span = data.Span;
        var type = PsxBeatmaniaFileType.Unknown;

        if (DetectChart(span))
            type = PsxBeatmaniaFileType.Chart;
        else if (DetectKeysound(span))
            type = PsxBeatmaniaFileType.Keysound;
        else if (DetectKst(span))
            type = PsxBeatmaniaFileType.Kst;
        else if (DetectDat3(span))
            type = PsxBeatmaniaFileType.Dat3;

        return new PsxBeatmaniaFile
        {
            Data = data,
            Type = type
        };
    }

    /// <inheritdoc />
    public List<PsxBeatmaniaSysFile> DecodeSysData(Stream source, long length)
    {
        return [];
    }
}