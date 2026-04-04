using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Compressions.BemaniLz.Processors;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Games.Beatmania.Ps2.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

[Service]
public class BeatmaniaPs2SongInfoDecoder(
    IBeatmaniaPs2OldChartStreamReader oldChartStreamReader,
    IBemaniLzDecoder bemaniLzDecoder,
    IBeatmaniaPs2ChartDecoder chartDecoder)
    : IBeatmaniaPs2SongInfoDecoder
{
    public List<BeatmaniaPs2SongInfo> Decode(ReadOnlySpan<byte> data, int songInfoOffset, BeatmaniaPs2FormatType type)
    {
        var temp = data.ToArray();
        var result = new List<BeatmaniaPs2SongInfo>();
        var offset = songInfoOffset;

        while (data[offset..].AsS32L() != 0)
        {
            var decoded = DecodeOne(data[offset..], type);
            if (decoded == null)
                break;
            
            offset += decoded.InfoSize;

            if (decoded.NameRef > 0)
            {
                decoded.Name = GetString(data[decoded.NameRef..]);
                decoded.NameRef = 0;
            }

            foreach (var difficulty in decoded.Difficulties)
            {
                if (difficulty.ChartRef > 0)
                {
                    var decompressedChart = bemaniLzDecoder.Decode(
                        new ReadOnlyMemoryStream(temp.AsMemory(difficulty.ChartRef))
                    );

                    var decodedChart = chartDecoder.Decode(oldChartStreamReader.Read(
                        new ReadOnlyMemoryStream(decompressedChart), decompressedChart.Length
                    ));

                    decodedChart[NumericData.SourceOffset] = difficulty.ChartRef;
                    difficulty.Chart = decodedChart;
                    difficulty.ChartRef = 0;
                }
            }

            result.Add(decoded);
        }

        return result;
    }

    private static BeatmaniaPs2SongInfo? DecodeOne(ReadOnlySpan<byte> data, BeatmaniaPs2FormatType type) =>
        type switch
        {
            BeatmaniaPs2FormatType.IIDX3rd => Decode2dx3rd(data),
            BeatmaniaPs2FormatType.IIDX4th => Decode2dx4th(data),
            BeatmaniaPs2FormatType.IIDX5th => Decode2dx5th(data),
            BeatmaniaPs2FormatType.IIDX6th => Decode2dx6th(data),
            BeatmaniaPs2FormatType.IIDX7th => Decode2dx7th(data),
            BeatmaniaPs2FormatType.IIDX8th => Decode2dx8th(data),
            BeatmaniaPs2FormatType.IIDX9th or BeatmaniaPs2FormatType.IIDX10th => Decode2dx9th(data),
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

    private static BeatmaniaPs2SongInfo? Decode2dx3rd(ReadOnlySpan<byte> data)
    {
        var myData = data[..0x07C];
        var nameRef = ReadInt32LittleEndian(myData[0x004..]) - 0xFF000;
        var difficulties = myData[0x00C..0x010].ToArray();
        var movies = GetShorts(myData[0x00A..0x00C]);
        var charts = GetInts(myData[0x054..0x06C]);
        var sets = GetShorts(myData[0x06C..0x078]);

        if (nameRef <= 0)
            nameRef = ReadInt32LittleEndian(myData) - 0xFF000;

        if (nameRef <= 0 || nameRef >= data.Length)
            return null;

        var result = new BeatmaniaPs2SongInfo
        {
            InfoSize = myData.Length,
            NameRef = nameRef,
            Difficulties = new[]
            {
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 1,
                    Level = difficulties[1],
                    Keysounds = sets[0],
                    Bgm = sets[2],
                    Movie = movies[0],
                    ChartRef = charts[0] - 0xFF000
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 2,
                    Level = difficulties[1],
                    Keysounds = sets[1],
                    Bgm = sets[3],
                    Movie = movies[0],
                    ChartRef = charts[1] - 0xFF000
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 1,
                    Level = difficulties[0],
                    Keysounds = sets[0],
                    Bgm = sets[4],
                    Movie = movies[0],
                    ChartRef = charts[2] - 0xFF000
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 2,
                    Level = difficulties[0],
                    Keysounds = sets[0],
                    Bgm = sets[4],
                    Movie = movies[0],
                    ChartRef = charts[3] - 0xFF000
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 1,
                    Level = difficulties[2],
                    Keysounds = sets[0],
                    Bgm = sets[2],
                    Movie = movies[0],
                    ChartRef = charts[4] - 0xFF000
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 2,
                    Level = difficulties[2],
                    Keysounds = sets[1],
                    Bgm = sets[3],
                    Movie = movies[0],
                    ChartRef = charts[5] - 0xFF000
                }
            }.Where(x => x.ChartRef > 0).ToList()
        };
        
        return result;
    }

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

    private static BeatmaniaPs2SongInfo Decode2dx9th(ReadOnlySpan<byte> data)
    {
        var myData = data[..0x16C];
        var name = GetString(myData[..0x040]).Trim();
        var difficulties = myData[0x04A..0x04F].ToArray();
        var movies = GetShorts(myData[0x056..0x05E]);
        var charts = GetInts(myData[0x12C..0x14C]);
        var sets = GetShorts(myData[0x14C..0x16C]);

        var result = new BeatmaniaPs2SongInfo
        {
            InfoSize = myData.Length,
            Name = name,
            Difficulties = new[]
            {
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 1,
                    Level = difficulties[0],
                    Keysounds = sets[0],
                    Bgm = sets[2],
                    Movie = movies[0],
                    ChartId = charts[0]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 2,
                    Level = difficulties[2],
                    Keysounds = sets[1],
                    Bgm = sets[3],
                    Movie = movies[0],
                    ChartId = charts[1]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 1,
                    Level = difficulties[0],
                    Keysounds = sets[4],
                    Bgm = sets[6],
                    Movie = movies[0],
                    ChartId = charts[2]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 2,
                    Level = difficulties[2],
                    Keysounds = sets[5],
                    Bgm = sets[7],
                    Movie = movies[0],
                    ChartId = charts[3]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 1,
                    Level = difficulties[1],
                    Keysounds = sets[8],
                    Bgm = sets[10],
                    Movie = movies[0],
                    ChartId = charts[4]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 2,
                    Level = difficulties[3],
                    Keysounds = sets[9],
                    Bgm = sets[11],
                    Movie = movies[0],
                    ChartId = charts[5]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Beginner",
                    Players = 1,
                    Level = difficulties[4],
                    Keysounds = sets[12],
                    Bgm = sets[14],
                    Movie = movies[0],
                    ChartId = charts[6]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Beginner",
                    Players = 2,
                    Level = difficulties[4],
                    Keysounds = sets[13],
                    Bgm = sets[15],
                    Movie = movies[0],
                    ChartId = charts[7]
                },
            }.Where(x => x.ChartId > 0).ToList()
        };
        
        return result;
    }

    private static BeatmaniaPs2SongInfo Decode2dx11th(ReadOnlySpan<byte> data)
    {
        var myData = data[..0x140];
        var name = GetString(myData[..0x040]).Trim();
        var difficulties = myData[0x4C..0x54].ToArray();
        var movies = GetShorts(myData[0x05A..0x062]);
        var charts = GetInts(myData[0x100..0x120]);
        var sets = GetShorts(myData[0x120..0x140]);

        var result = new BeatmaniaPs2SongInfo
        {
            InfoSize = myData.Length,
            Name = name,
            Difficulties = new[]
            {
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 1,
                    Level = difficulties[0],
                    Keysounds = sets[0],
                    Bgm = sets[2],
                    Movie = movies[0],
                    ChartId = charts[0]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 2,
                    Level = difficulties[2],
                    Keysounds = sets[1],
                    Bgm = sets[3],
                    Movie = movies[0],
                    ChartId = charts[1]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 1,
                    Level = difficulties[0],
                    Keysounds = sets[4],
                    Bgm = sets[6],
                    Movie = movies[0],
                    ChartId = charts[2]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 2,
                    Level = difficulties[2],
                    Keysounds = sets[5],
                    Bgm = sets[7],
                    Movie = movies[0],
                    ChartId = charts[3]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 1,
                    Level = difficulties[1],
                    Keysounds = sets[8],
                    Bgm = sets[10],
                    Movie = movies[0],
                    ChartId = charts[4]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 2,
                    Level = difficulties[3],
                    Keysounds = sets[9],
                    Bgm = sets[11],
                    Movie = movies[0],
                    ChartId = charts[5]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Beginner",
                    Players = 1,
                    Level = difficulties[4],
                    Keysounds = sets[12],
                    Bgm = sets[14],
                    Movie = movies[0],
                    ChartId = charts[6]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Beginner",
                    Players = 2,
                    Level = difficulties[4],
                    Keysounds = sets[13],
                    Bgm = sets[15],
                    Movie = movies[0],
                    ChartId = charts[7]
                },
            }.Where(x => x.ChartId > 0).ToList()
        };
        
        return result;
    }

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
        var myData = data[..0x174];
        var name = GetString(myData[..0x040]).Trim();
        var difficulties = myData[0x04A..0x052].ToArray();
        var movies = GetShorts(myData[0x05A..0x062]);
        var charts = GetInts(myData[0x130..0x150]);
        var sets = GetShorts(myData[0x150..0x170]);
        var fiveKey = myData[0x170..0x174].AsS32L() == 0;

        var result = new BeatmaniaPs2SongInfo
        {
            InfoSize = myData.Length,
            Name = name,
            IsFiveKey = fiveKey,
            Difficulties = new[]
            {
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 1,
                    Level = difficulties[0],
                    Keysounds = sets[0],
                    Bgm = sets[2],
                    Movie = movies[0],
                    ChartId = charts[0]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Normal",
                    Players = 2,
                    Level = difficulties[4],
                    Keysounds = sets[1],
                    Bgm = sets[3],
                    Movie = movies[0],
                    ChartId = charts[1]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 1,
                    Level = difficulties[1],
                    Keysounds = sets[4],
                    Bgm = sets[6],
                    Movie = movies[0],
                    ChartId = charts[2]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Hyper",
                    Players = 2,
                    Level = difficulties[5],
                    Keysounds = sets[5],
                    Bgm = sets[7],
                    Movie = movies[0],
                    ChartId = charts[3]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 1,
                    Level = difficulties[2],
                    Keysounds = sets[8],
                    Bgm = sets[10],
                    Movie = movies[0],
                    ChartId = charts[4]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Another",
                    Players = 2,
                    Level = difficulties[6],
                    Keysounds = sets[9],
                    Bgm = sets[11],
                    Movie = movies[0],
                    ChartId = charts[5]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Beginner",
                    Players = 1,
                    Level = difficulties[3],
                    Keysounds = sets[12],
                    Bgm = sets[14],
                    Movie = movies[0],
                    ChartId = charts[6]
                },
                new BeatmaniaPs2DifficultyInfo
                {
                    Name = "Beginner",
                    Players = 2,
                    Level = difficulties[7],
                    Keysounds = sets[13],
                    Bgm = sets[15],
                    Movie = movies[0],
                    ChartId = charts[7]
                },
            }.Where(x => x.ChartId > 0).ToList()
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