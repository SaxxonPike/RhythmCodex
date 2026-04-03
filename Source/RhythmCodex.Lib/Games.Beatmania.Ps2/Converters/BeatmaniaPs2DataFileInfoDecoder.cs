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
            BeatmaniaPs2FormatType.IIDX3rd or 
                BeatmaniaPs2FormatType.IIDX4th => Decode3rd(data),
            BeatmaniaPs2FormatType.IIDX5th or
                BeatmaniaPs2FormatType.IIDX6th or 
                BeatmaniaPs2FormatType.IIDX7th or
                BeatmaniaPs2FormatType.IIDX8th => Decode5th(data),
            BeatmaniaPs2FormatType.IIDX9th or
                BeatmaniaPs2FormatType.IIDX10th or
                BeatmaniaPs2FormatType.IIDX11th or
                BeatmaniaPs2FormatType.IIDX12th or
                BeatmaniaPs2FormatType.IIDX13th or
                BeatmaniaPs2FormatType.US => Decode9th(data),
            BeatmaniaPs2FormatType.IIDX14th or 
                BeatmaniaPs2FormatType.IIDX15th or 
                BeatmaniaPs2FormatType.IIDX16th or 
                BeatmaniaPs2FormatType.IIDXPremiumBest => Decode14th(data),
            _ => throw new RhythmCodexException("Unsupported format.")
        };

    private static BeatmaniaPs2DataFileInfo Decode3rd(ReadOnlySpan<byte> data) =>
        new()
        {
            InfoSize = 12,
            Offset = unchecked(data.AsU32L() * 0x800U),
            Size = unchecked(data[4..].AsU32L())
        };

    private static BeatmaniaPs2DataFileInfo Decode5th(ReadOnlySpan<byte> data) =>
        new()
        {
            InfoSize = 16,
            Offset = unchecked(data.AsU32L() * 0x800U),
            Size = unchecked(data[8..].AsU32L())
        };

    private static BeatmaniaPs2DataFileInfo Decode9th(ReadOnlySpan<byte> data) =>
        new()
        {
            InfoSize = 8,
            Offset = unchecked(data.AsU32L() * 0x800U),
            Size = unchecked(data[4..].AsU32L() * 0x800U)
        };
    
    private static BeatmaniaPs2DataFileInfo Decode14th(ReadOnlySpan<byte> data) =>
        new()
        {
            InfoSize = 12,
            Offset = unchecked(data.AsU32L() * 0x800U),
            Size = unchecked(data[8..].AsU32L())
        };
}