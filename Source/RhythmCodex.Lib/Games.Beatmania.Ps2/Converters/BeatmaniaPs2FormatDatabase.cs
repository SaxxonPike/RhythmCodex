using System;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <inheritdoc />
[Service]
public sealed class BeatmaniaPs2FormatDatabase : IBeatmaniaPs2FormatDatabase
{
    /// <inheritdoc />
    public BeatmaniaPs2FormatType? GetTypeByExeName(string name) =>
        name.ToUpperInvariant() switch
        {
            "SLPM_650.06" => BeatmaniaPs2FormatType.IIDX3rd,
            "SLPM_650.26" => BeatmaniaPs2FormatType.IIDX4th,
            "SLPM_650.49" => BeatmaniaPs2FormatType.IIDX5th,
            "SLPM_651.56" => BeatmaniaPs2FormatType.IIDX6th,
            "SLPM_655.93" => BeatmaniaPs2FormatType.IIDX7th,
            "SLPM_657.68" => BeatmaniaPs2FormatType.IIDX8th,
            "SLPM_659.46" => BeatmaniaPs2FormatType.IIDX9th,
            "SLPM_661.80" => BeatmaniaPs2FormatType.IIDX10th,
            "SLPM_664.26" => BeatmaniaPs2FormatType.IIDX11th,
            "SLPM_666.21" => BeatmaniaPs2FormatType.IIDX12th,
            "SLPM_668.28" => BeatmaniaPs2FormatType.IIDX13th,
            "SLPM_669.95" => BeatmaniaPs2FormatType.IIDX14th,
            "SLPM_551.17" => BeatmaniaPs2FormatType.IIDX15th,
            "SLUS_212.39" => BeatmaniaPs2FormatType.US,
            _ => throw new ArgumentException($"Unknown executable name: {name}", nameof(name))
        };

    /// <inheritdoc />
    public BeatmaniaPs2FormatInfo? GetFormatByType(BeatmaniaPs2FormatType type) =>
        type switch
        {
            BeatmaniaPs2FormatType.IIDX3rd => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_650.06",
                        FileTableOffset = 0x145CD0,
                        SongTableOffset = 0x77FC8,
                        BlobFileName = "DX2_3/BM2DX3.BIN"
                    }
                ],
                UseOldReaders = true
            },
            BeatmaniaPs2FormatType.IIDX4th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_650.26",
                        FileTableOffset = 0x137450,
                        SongTableOffset = 0x8BC98,
                        BlobFileName = "DX2_4/BM2DX4.BIN"
                    }
                ],
                UseOldReaders = true
            },
            BeatmaniaPs2FormatType.IIDX5th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_650.49",
                        FileTableOffset = 0x1837D8,
                        SongTableOffset = 0xAE520,
                        BlobFileName = "DX2_5/BM2DX5.BIN"
                    }
                ],
                UseOldReaders = true
            },
            BeatmaniaPs2FormatType.IIDX6th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_651.56",
                        FileTableOffset = 0x180058,
                        SongTableOffset = 0x1885B8,
                        BlobFileName = "DX2_6/BM2DX6A.BIN"
                    },
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_651.56",
                        FileTableOffset = 0x1815E8,
                        SongTableOffset = 0x1885B8,
                        BlobFileName = "DX2_6/BM2DX6B.BIN"
                    }
                ],
                UseOldReaders = true
            },
            BeatmaniaPs2FormatType.IIDX7th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_655.93",
                        FileTableOffset = 0x1B7460,
                        SongTableOffset = 0x1C1AF0,
                        BlobFileName = "DX2_7/BM2DX7B.BIN"
                    },
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_655.93",
                        FileTableOffset = 0x1B9A30,
                        SongTableOffset = 0x1C1AF0,
                        BlobFileName = "DX2_7/BM2DX7C.BIN"
                    }
                ]
            },
            BeatmaniaPs2FormatType.IIDX8th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_657.68",
                        FileTableOffset = 0x19A940,
                        SongTableOffset = 0x1A4060,
                        BlobFileName = "DX2_8/BM2DX8B.BIN"
                    },
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_657.68",
                        FileTableOffset = 0x19B080,
                        SongTableOffset = 0x1A4060,
                        BlobFileName = "DX2_8/BM2DX8C.BIN"
                    }
                ]
            },
            BeatmaniaPs2FormatType.IIDX9th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_659.46",
                        FileTableOffset = 0xBD230,
                        SongTableOffset = 0xC1500,
                        BlobFileName = "DATA2.DAT"
                    }
                ]
            },
            BeatmaniaPs2FormatType.IIDX10th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_661.80",
                        FileTableOffset = 0xCDC90,
                        SongTableOffset = 0x10BAE0,
                        BlobFileName = "DATA2.DAT"
                    }
                ]
            },
            BeatmaniaPs2FormatType.IIDX11th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_664.26",
                        FileTableOffset = 0xEE440,
                        SongTableOffset = 0x1C21F0,
                        BlobFileName = "DATA2.DAT"
                    }
                ]
            },
            BeatmaniaPs2FormatType.IIDX12th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_666.21",
                        FileTableOffset = 0x105340,
                        SongTableOffset = 0x115F10,
                        BlobFileName = "BM2DX12B.BIN",
                        BaseIndex = 28
                    },
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_666.21",
                        FileTableOffset = 0x106A40,
                        SongTableOffset = 0x115F10,
                        BlobFileName = "BM2DX12C.BIN",
                        BaseIndex = 764
                    }
                ]
            },
            BeatmaniaPs2FormatType.IIDX13th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_668.28",
                        FileTableOffset = 0x112AC0,
                        SongTableOffset = 0x1353E0,
                        BlobFileName = "BM2DX13B.BIN",
                        BaseIndex = 24
                    },
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_668.28",
                        FileTableOffset = 0x1143C0,
                        SongTableOffset = 0x1353E0,
                        BlobFileName = "BM2DX13C.BIN",
                        BaseIndex = 824
                    }
                ]
            },
            BeatmaniaPs2FormatType.IIDX14th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_669.95",
                        FileTableOffset = 0x11AD60,
                        SongTableOffset = 0x156300,
                        BlobFileName = "BM2DX14B.BIN"
                    },
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_669.95",
                        FileTableOffset = 0x11D64C,
                        SongTableOffset = 0x156300,
                        BlobFileName = "BM2DX14C.BIN",
                        BaseIndex = 873
                    }
                ]
            },
            BeatmaniaPs2FormatType.IIDX15th => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_551.17",
                        FileTableOffset = 0x134020,
                        SongTableOffset = 0x16FE60,
                        BlobFileName = "BM2DX15A.BIN"
                    },
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_551.17",
                        FileTableOffset = 0x134260,
                        SongTableOffset = 0x16FE60,
                        BlobFileName = "BM2DX15B.BIN",
                        BaseIndex = 48
                    },
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLPM_551.17",
                        FileTableOffset = 0x136A20,
                        SongTableOffset = 0x16FE60,
                        BlobFileName = "BM2DX15C.BIN",
                        BaseIndex = 896
                    }
                ]
            },
            BeatmaniaPs2FormatType.US => new BeatmaniaPs2FormatInfo
            {
                MetaTables =
                [
                    new BeatmaniaPs2FormatMetaTable
                    {
                        BinaryFileName = "SLUS_212.39",
                        FileTableOffset = 0xBA710,
                        SongTableOffset = 0xBF510,
                        BlobFileName = "DATA2.DAT"
                    }
                ]
            },
            _ => throw new RhythmCodexException("Unknown Beatmania PS2 format type")
        };
}