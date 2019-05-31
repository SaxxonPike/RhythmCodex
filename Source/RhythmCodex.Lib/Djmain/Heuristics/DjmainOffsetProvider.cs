using System.Collections.Generic;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Heuristics
{
    [Service]
    public class DjmainOffsetProvider : IDjmainOffsetProvider
    {
        public IList<int> GetSampleChartMap(DjmainChunkFormat format)
        {
            switch (format)
            {
                case DjmainChunkFormat.Unknown:
                    throw new RhythmCodexException("Can't get sample chart map for unknown format.");
                case DjmainChunkFormat.BeatmaniaFinal:
                    return new List<int> {0, 0, 0, 1, 1, 1};
                case DjmainChunkFormat.BeatmaniaFirst:
                case DjmainChunkFormat.BeatmaniaSecond:
                    return new List<int> {0};
                default:
                    return new List<int> {0, 0, 0};
            }
        }
        
        public ICollection<int> GetChartOffsets(DjmainChunkFormat format)
        {
            switch (format)
            {
                case DjmainChunkFormat.Unknown:
                    throw new RhythmCodexException("Can't get chart offsets for unknown format.");
                case DjmainChunkFormat.BeatmaniaFirst:
                case DjmainChunkFormat.BeatmaniaSecond:
                    return new List<int> {0x000400};
                case DjmainChunkFormat.BeatmaniaThird:
                case DjmainChunkFormat.BeatmaniaComplete:
                    return new List<int> {0x000400, 0xF02000, 0xF03000};
                case DjmainChunkFormat.BeatmaniaFinal:
                    return new List<int> {0x002000, 0x006000, 0x00A000, 0x00E000, 0x012000, 0x016000};
                default:
                    return new List<int> {0x000800, 0xF02000, 0xF03000};
            }
        }

        public int GetSoundOffset(DjmainChunkFormat format)
        {
            switch (format)
            {
                case DjmainChunkFormat.Unknown:
                    throw new RhythmCodexException("Can't get sound offsets for unknown format.");
                case DjmainChunkFormat.BeatmaniaFinal:
                    return 0x020000;
                default:
                    return 0x002000;
            }
        }

        public ICollection<int> GetSampleMapOffsets(DjmainChunkFormat format)
        {
            switch (format)
            {
                case DjmainChunkFormat.Unknown:
                    throw new RhythmCodexException("Can't get sample map offsets for unknown format.");
                case DjmainChunkFormat.BeatmaniaFirst:
                case DjmainChunkFormat.BeatmaniaSecond:
                case DjmainChunkFormat.BeatmaniaThird:
                case DjmainChunkFormat.BeatmaniaComplete:
                    return new List<int> {0x000000, 0x000200};
                case DjmainChunkFormat.BeatmaniaFinal:
                    return new List<int> {0x000000, 0x001000};
                default:
                    return new List<int> {0x000000, 0x000400};
            }
        }

        public ICollection<string> GetChartNames(DjmainChunkFormat format)
        {
            switch (format)
            {
                case DjmainChunkFormat.Unknown:
                    throw new RhythmCodexException("Can't get chart names for unknown format.");
                case DjmainChunkFormat.BeatmaniaFirst:
                case DjmainChunkFormat.BeatmaniaSecond:
                    return new List<string>
                    {
                        BeatmaniaDifficultyConstants.Normal
                    };
                case DjmainChunkFormat.BeatmaniaFinal:
                    return new List<string>
                    {
                        BeatmaniaDifficultyConstants.Normal, 
                        BeatmaniaDifficultyConstants.Light, 
                        BeatmaniaDifficultyConstants.Another,
                        BeatmaniaDifficultyConstants.Normal,
                        BeatmaniaDifficultyConstants.Light,
                        BeatmaniaDifficultyConstants.Another
                    };
                default:
                    return new List<string>
                    {
                        BeatmaniaDifficultyConstants.Normal, 
                        BeatmaniaDifficultyConstants.Light, 
                        BeatmaniaDifficultyConstants.Another,
                    };
            }
        }

        
    }
}