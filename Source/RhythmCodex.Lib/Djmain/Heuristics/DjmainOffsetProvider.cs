using System.Collections.Generic;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Djmain.Heuristics
{
    [Service]
    public class DjmainOffsetProvider : IDjmainOffsetProvider
    {
        public ICollection<int> GetChartOffsets(DjmainChunkFormat format)
        {
            switch (format)
            {
                case DjmainChunkFormat.Unknown:
                    throw new RhythmCodexException("Can't get chart offsets for unknown format.");
                case DjmainChunkFormat.First:
                case DjmainChunkFormat.Second:
                    return new List<int> {0x000400};
                case DjmainChunkFormat.Third:
                case DjmainChunkFormat.Complete:
                    return new List<int> {0x000400, 0xF02000, 0xF03000};
                case DjmainChunkFormat.Final:
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
                case DjmainChunkFormat.Final:
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
                case DjmainChunkFormat.First:
                case DjmainChunkFormat.Second:
                case DjmainChunkFormat.Third:
                case DjmainChunkFormat.Complete:
                    return new List<int> {0x000000, 0x000200};
                case DjmainChunkFormat.Final:
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
                case DjmainChunkFormat.First:
                case DjmainChunkFormat.Second:
                    return new List<string> {"normal"};
                case DjmainChunkFormat.Final:
                    return new List<string> {"normal", "light", "another", "normal", "light", "another"};
                default:
                    return new List<string> {"normal", "light", "another"};
            }
        }

        
    }
}