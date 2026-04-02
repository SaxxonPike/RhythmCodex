using System;
using System.Collections.Generic;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

[Service]
public sealed class BeatmaniaPs2DataFileInfoDecoder : IBeatmaniaPs2DataFileInfoDecoder
{
    public List<BeatmaniaPs2DataFileInfo> Decode(
        ReadOnlySpan<byte> data, 
        int dataFileInfoOffset, 
        BeatmaniaPs2FormatType type)
    {
        var offset = dataFileInfoOffset;
        var lastOffset = -1L;
        var result = new List<BeatmaniaPs2DataFileInfo>();

        while (offset < data.Length)
        {
            var decoded = DecodeOne(data[offset..], type);
            offset += decoded.InfoSize;

            if (decoded.Size <= 0 || decoded.Offset < 0 || decoded.Offset < lastOffset)
                break;

            result.Add(decoded);
            lastOffset = decoded.Offset;
        }

        return result;
    }

    private static BeatmaniaPs2DataFileInfo DecodeOne(ReadOnlySpan<byte> data, BeatmaniaPs2FormatType type) =>
        type switch
        {
            BeatmaniaPs2FormatType.US => Decode8Byte800(data),
            _ => throw new RhythmCodexException("Unsupported format.")
        };

    private static BeatmaniaPs2DataFileInfo Decode8Byte800(ReadOnlySpan<byte> data) =>
        new()
        {
            InfoSize = 8,
            Offset = unchecked(data.AsU32L() * 0x800U),
            Size = unchecked(data[4..].AsU32L() * 0x800U)
        };
}