using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

[Service]
public class BeatmaniaPs2SongInfoDecoder(IBeatmaniaPs2FormatDatabase beatmaniaPs2FormatDatabase)
    : IBeatmaniaPs2SongInfoDecoder
{
    public List<BeatmaniaPs2SongInfo> Decode(ReadOnlySpan<byte> data, int songInfoOffset, BeatmaniaPs2FormatType type)
    {
        var result = new List<BeatmaniaPs2SongInfo>();
        var offset = songInfoOffset;

        while (data[offset..].AsS32L() != 0)
        {
            var decoded = DecodeOne(data[offset..], type);
            offset += decoded.InfoSize;

            if (decoded.NameRef > 0)
            {
                decoded.Name = GetString(data[decoded.NameRef..]);
                decoded.NameRef = 0;
            }

            result.Add(decoded);
        }

        return result;
    }

    private static BeatmaniaPs2SongInfo DecodeOne(ReadOnlySpan<byte> data, BeatmaniaPs2FormatType type) =>
        type switch
        {
            BeatmaniaPs2FormatType.IIDX3rd => Decode2dx3rd(data),
            BeatmaniaPs2FormatType.IIDX4th => Decode2dx4th(data),
            BeatmaniaPs2FormatType.IIDX5th => Decode2dx5th(data),
            BeatmaniaPs2FormatType.IIDX6th => Decode2dx6th(data),
            BeatmaniaPs2FormatType.IIDX7th => Decode2dx7th(data),
            BeatmaniaPs2FormatType.IIDX8th => Decode2dx8th(data),
            BeatmaniaPs2FormatType.IIDX9th => Decode2dx9th(data),
            BeatmaniaPs2FormatType.IIDX10th => Decode2dx10th(data),
            BeatmaniaPs2FormatType.IIDX11th => Decode2dx11th(data),
            BeatmaniaPs2FormatType.IIDX12th => Decode2dx12th(data),
            BeatmaniaPs2FormatType.IIDX13th => Decode2dx13th(data),
            BeatmaniaPs2FormatType.IIDX14th => Decode2dx14th(data),
            BeatmaniaPs2FormatType.IIDX15th => Decode2dx15th(data),
            BeatmaniaPs2FormatType.IIDX16th => Decode2dx16th(data),
            BeatmaniaPs2FormatType.IIDXPremiumBest => Decode2dxPremiumBest(data),
            BeatmaniaPs2FormatType.US => Decode2dxUs(data),
            _ => throw new RhythmCodexException("Unrecognized format.")
        };

    private static BeatmaniaPs2SongInfo Decode2dx3rd(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx4th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx5th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx6th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx7th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx8th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx9th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx10th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx11th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx12th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx13th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx14th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx15th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dx16th(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dxPremiumBest(ReadOnlySpan<byte> data) =>
        throw new NotImplementedException();

    private static BeatmaniaPs2SongInfo Decode2dxUs(ReadOnlySpan<byte> data)
    {
        var name = GetString(data[..0x040]).Trim();
        var difficulties = data[0x04A..0x052].ToArray();
        var movies = GetShorts(data[0x05A..0x062]);
        var charts = GetInts(data[0x130..0x150]);
        var sets = GetShorts(data[0x150..0x170]);
        var fiveKey = data[0x170..0x174].AsS32L() == 0;

        var result = new BeatmaniaPs2SongInfo
        {
            InfoSize = 0x174,
            Name = name,
            IsFiveKey = fiveKey,
            Difficulties = new[]
            {
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 1,
                    Level = difficulties[0],
                    Keysound = sets[0],
                    Bgm = sets[2],
                    Movie = movies[0],
                    Chart = charts[0]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 2,
                    Level = difficulties[4],
                    Keysound = sets[1],
                    Bgm = sets[3],
                    Movie = movies[0],
                    Chart = charts[1]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 1,
                    Level = difficulties[1],
                    Keysound = sets[4],
                    Bgm = sets[6],
                    Movie = movies[0],
                    Chart = charts[2]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 2,
                    Level = difficulties[5],
                    Keysound = sets[5],
                    Bgm = sets[7],
                    Movie = movies[0],
                    Chart = charts[3]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 1,
                    Level = difficulties[2],
                    Keysound = sets[8],
                    Bgm = sets[10],
                    Movie = movies[0],
                    Chart = charts[4]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 2,
                    Level = difficulties[6],
                    Keysound = sets[9],
                    Bgm = sets[11],
                    Movie = movies[0],
                    Chart = charts[5]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Beginner",
                    Players = 1,
                    Level = difficulties[3],
                    Keysound = sets[12],
                    Bgm = sets[14],
                    Movie = movies[0],
                    Chart = charts[6]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Beginner",
                    Players = 2,
                    Level = difficulties[7],
                    Keysound = sets[13],
                    Bgm = sets[15],
                    Movie = movies[0],
                    Chart = charts[7]
                },
            }.Where(x => x.Chart > 0).ToList()
        };
        
        return result;
    }

    private static short[] GetShorts(ReadOnlySpan<byte> bytes)
    {
        var result = new short[bytes.Length / sizeof(short)];
        for (var i = 0; i < result.Length; i++)
            result[i] = bytes[(i * sizeof(short))..].AsS16L();
        return result;
    }

    private static int[] GetInts(ReadOnlySpan<byte> bytes)
    {
        var result = new int[bytes.Length / sizeof(int)];
        for (var i = 0; i < result.Length; i++)
            result[i] = bytes[(i * sizeof(int))..].AsS32L();
        return result;
    }

    private static string GetString(ReadOnlySpan<byte> bytes)
    {
        var zeroOffset = bytes.IndexOf((byte)0x00);
        if (zeroOffset < 0)
            zeroOffset = bytes.Length;

        return Encodings.Cp932.GetString(bytes[..zeroOffset]);
    }
}