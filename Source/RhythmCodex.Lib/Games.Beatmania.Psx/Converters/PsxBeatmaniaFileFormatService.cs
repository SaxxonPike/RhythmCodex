using System;
using RhythmCodex.Games.Beatmania.Psx.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Beatmania.Psx.Converters;

/// <inheritdoc />
[Service]
public sealed class PsxBeatmaniaFileFormatService : IPsxBeatmaniaFileFormatService
{
    /// <inheritdoc />
    public PsxBeatmaniaFileType DetectFormat(ReadOnlySpan<byte> data)
    {
        var type = PsxBeatmaniaFileType.Unknown;

        if (DetectChart(data))
            type = PsxBeatmaniaFileType.Chart;
        else if (DetectKeysound(data))
            type = PsxBeatmaniaFileType.Keysound;
        else if (DetectKst(data))
            type = PsxBeatmaniaFileType.Kst;
        else if (DetectDat3(data))
            type = PsxBeatmaniaFileType.Dat3;
        else if (DetectGfx(data))
            type = PsxBeatmaniaFileType.Graphics;
        else if (DetectScript(data))
            type = PsxBeatmaniaFileType.Script;

        return type;
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
    /// Returns true if the data is a GFX file.
    /// </summary>
    private static bool DetectGfx(ReadOnlySpan<byte> span) =>
        span.Length >= 12 &&
        ReadInt32LittleEndian(span) < span.Length &&
        ReadInt32LittleEndian(span[8..]) == 0 &&
        span[^1] == 0xFF;

    /// <summary>
    /// Returns true if the data is a script file.
    /// </summary>
    private static bool DetectScript(ReadOnlySpan<byte> span)
    {
        if (span.Length < 64)
            return false;

        var offsetBytes = span[..64];
        var lastOffset = -1;

        for (var i = 0; i < 16; i++)
        {
            var thisOffset = ReadInt32LittleEndian(offsetBytes[(i << 2)..]);
            if (thisOffset <= lastOffset)
                return false;
            lastOffset = thisOffset;
        }

        return true;
    }
}