using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Archs.Djmain.Converters;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Archs.Djmain.Streamers;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Psx.Models;
using RhythmCodex.Games.Beatmania.Psx.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Beatmania.Psx.Converters;

/// <inheritdoc />
[Service]
public sealed class PsxBeatmaniaDecoder(
    IPsxBeatmaniaFileStreamReader fileStreamReader,
    IDjmainChartEventStreamReader chartEventStreamReader,
    IDjmainChartDecoder chartDecoder,
    IPsxBeatmaniaFileFormatService fileFormatService
) : IPsxBeatmaniaDecoder
{
    /// <inheritdoc />
    public Chart DecodeChart(Stream source, DjmainDecodeOptions options)
    {
        var events = chartEventStreamReader.Read(source);
        var chart = chartDecoder.Decode(events, options with
        {
            ChartType = DjmainChartType.BeatmaniaCs,
            SwapStereo = false
        });
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
                    var bmDataPakDirectory = fileStreamReader.ReadDirectory(pak);
                    var bmFiles = fileStreamReader.ReadEntries(pak, bmDataPakDirectory);
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
    /// Determines the type of the data and returns a <see cref="PsxBeatmaniaFile"/>
    /// populated with the detected type.
    /// </summary>
    private PsxBeatmaniaFile CreateFile(ReadOnlyMemory<byte> data)
    {
        var type = fileFormatService.DetectFormat(data.Span);

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