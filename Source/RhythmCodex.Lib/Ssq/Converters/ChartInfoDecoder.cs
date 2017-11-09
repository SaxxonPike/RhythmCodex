using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class ChartInfoDecoder : IChartInfoDecoder
    {
        private readonly ILogger _logger;

        public ChartInfoDecoder(ILogger logger)
        {
            _logger = logger;
        }
        
        public ChartInfo Decode(int param1)
        {
            return new ChartInfo
            {
                Difficulty = GetDifficulty(param1),
                PanelCount = GetPanelCount(param1) ?? 4,
                PlayerCount = GetPlayerCount(param1) ?? 1,
                Type = GetType(param1)
            };
        }

        private string GetType(int param1)
        {
            switch (param1 & 0xFF)
            {
                case 0x14:
                    return "Single";
                case 0x16:
                    return "Solo";
                case 0x18:
                    return "Double";
                case 0x24:
                    return "Couple";
                default:
                    _logger.Warning($"Unrecognized chart type {param1 & 0xFF:X2}");
                    return null;
            }
        }

        private string GetDifficulty(int param1)
        {
            switch ((param1 >> 8) & 0xFF)
            {
                case 0x01:
                    return "Easy";
                case 0x02:
                    return "Medium";
                case 0x03:
                    return "Hard";
                case 0x04:
                    return "Beginner";
                case 0x06:
                    return "Challenge";
                case 0x10:
                    // TODO: Couple charts use this value. This doesn't seem right.
                    return "Medium";
                default:
                    _logger.Warning($"Unrecognized chart difficulty {(param1 >> 8) & 0xFF:X2}");
                    return "Edit";
            }
        }

        private int? GetPanelCount(int param1)
        {
            switch (param1 & 0xFF)
            {
                case 0x14:
                    return 4;
                case 0x16:
                    return 6;
                case 0x18:
                    return 4;
                case 0x24:
                    return 4;
                default:
                    _logger.Warning($"Unrecognized panel type {param1 & 0xFF:X2}");
                    return null;
            }
        }

        private int? GetPlayerCount(int param1)
        {
            switch (param1 & 0xFF)
            {
                case 0x14:
                    return 1;
                case 0x16:
                    return 1;
                case 0x18:
                    return 2;
                case 0x24:
                    return 2;
                default:
                    _logger.Warning($"Unrecognized player count {param1 & 0xFF:X2}");
                    return null;
            }
        }
    }
}