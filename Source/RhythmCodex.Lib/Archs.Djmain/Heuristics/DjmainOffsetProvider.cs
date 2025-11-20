using System.Collections.Generic;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Games.Beatmania.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Djmain.Heuristics;

[Service]
public class DjmainOffsetProvider : IDjmainOffsetProvider
{
    public List<int> GetSampleChartMap(DjmainChunkFormat format)
    {
        switch (format)
        {
            case DjmainChunkFormat.Unknown:
                throw new RhythmCodexException("Can't get sample chart map for unknown format.");
            case DjmainChunkFormat.BeatmaniaFinal:
                return [0, 0, 0, 1, 1, 1];
            case DjmainChunkFormat.BeatmaniaFirst:
            case DjmainChunkFormat.BeatmaniaSecond:
            case DjmainChunkFormat.Popn1:
                return [0];
            default:
                return [0, 0, 0];
        }
    }

    public List<int> GetChartOffsets(DjmainChunkFormat format)
    {
        switch (format)
        {
            case DjmainChunkFormat.Unknown:
                throw new RhythmCodexException("Can't get chart offsets for unknown format.");
            case DjmainChunkFormat.BeatmaniaFirst:
            case DjmainChunkFormat.BeatmaniaSecond:
                return [0x000400];
            case DjmainChunkFormat.BeatmaniaThird:
            case DjmainChunkFormat.BeatmaniaComplete:
                return [0x000400, 0xF02000, 0xF03000];
            case DjmainChunkFormat.BeatmaniaFinal:
                return [0x002000, 0x006000, 0x00A000, 0x00E000, 0x012000, 0x016000];
            case DjmainChunkFormat.Popn1:
                return [0x000800];
            default:
                return [0x000800, 0xF02000, 0xF03000];
        }
    }

    public int GetSoundOffset(DjmainChunkFormat format)
    {
        return format switch
        {
            DjmainChunkFormat.Unknown =>
                throw new RhythmCodexException("Can't get sound offsets for unknown format."),
            DjmainChunkFormat.BeatmaniaFinal => 0x020000,
            _ => 0x002000
        };
    }

    public List<int> GetSampleMapOffsets(DjmainChunkFormat format)
    {
        switch (format)
        {
            case DjmainChunkFormat.Unknown:
                throw new RhythmCodexException("Can't get sample map offsets for unknown format.");
            case DjmainChunkFormat.BeatmaniaFirst:
            case DjmainChunkFormat.BeatmaniaSecond:
            case DjmainChunkFormat.BeatmaniaThird:
            case DjmainChunkFormat.BeatmaniaComplete:
                return [0x000000, 0x000200];
            case DjmainChunkFormat.BeatmaniaFinal:
                return [0x000000, 0x001000];
            case DjmainChunkFormat.Popn1:
                return [0x000000];
            default:
                return [0x000000, 0x000400];
        }
    }

    public int GetSampleMapMaxSize(DjmainChunkFormat format)
    {
        switch (format)
        {
            case DjmainChunkFormat.Unknown:
                throw new RhythmCodexException("Can't get sample map offsets for unknown format.");
            case DjmainChunkFormat.BeatmaniaFirst:
            case DjmainChunkFormat.BeatmaniaSecond:
            case DjmainChunkFormat.BeatmaniaThird:
            case DjmainChunkFormat.BeatmaniaComplete:
                return 0x0400;
            case DjmainChunkFormat.BeatmaniaFinal:
                return 0x0B00;
            default:
                return 0x0800;
        }
    }

    public List<string> GetChartNames(DjmainChunkFormat format)
    {
        switch (format)
        {
            case DjmainChunkFormat.Unknown:
                throw new RhythmCodexException("Can't get chart names for unknown format.");
            case DjmainChunkFormat.BeatmaniaFirst:
            case DjmainChunkFormat.BeatmaniaSecond:
                return
                [
                    BeatmaniaDifficultyConstants.Normal
                ];
            case DjmainChunkFormat.BeatmaniaFinal:
                return
                [
                    BeatmaniaDifficultyConstants.Normal,
                    BeatmaniaDifficultyConstants.Light,
                    BeatmaniaDifficultyConstants.Another,
                    BeatmaniaDifficultyConstants.Normal,
                    BeatmaniaDifficultyConstants.Light,
                    BeatmaniaDifficultyConstants.Another
                ];
            default:
                return
                [
                    BeatmaniaDifficultyConstants.Normal,
                    BeatmaniaDifficultyConstants.Light,
                    BeatmaniaDifficultyConstants.Another
                ];
        }
    }
}