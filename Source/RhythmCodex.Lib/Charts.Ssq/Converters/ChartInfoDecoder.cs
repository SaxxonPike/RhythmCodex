using RhythmCodex.Charts.Ssq.Model;
using RhythmCodex.Games.Stepmania.Model;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Charts.Ssq.Converters;

[Service]
public class ChartInfoDecoder(ILogger logger) : IChartInfoDecoder
{
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
                return SmGameTypes.Single;
            case 0x16:
                return SmGameTypes.Solo;
            case 0x18:
                return SmGameTypes.Double;
            case 0x24:
                return SmGameTypes.Couple;
            default:
                logger.Warning($"Unrecognized chart type {param1 & 0xFF:X2}");
                return null;
        }
    }

    private string GetDifficulty(int param1)
    {
        switch ((param1 >> 8) & 0xFF)
        {
            case 0x01:
                return SmNotesDifficulties.Easy;
            case 0x02:
                return SmNotesDifficulties.Medium;
            case 0x03:
                return SmNotesDifficulties.Hard;
            case 0x04:
                return SmNotesDifficulties.Beginner;
            case 0x06:
                return SmNotesDifficulties.Challenge;
            case 0x10:
                // TODO: Couple charts use this value. This doesn't seem right.
                return SmNotesDifficulties.Medium;
            default:
                logger.Warning($"Unrecognized chart difficulty {(param1 >> 8) & 0xFF:X2}");
                return SmNotesDifficulties.Edit;
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
                logger.Warning($"Unrecognized panel type {param1 & 0xFF:X2}");
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
                logger.Warning($"Unrecognized player count {param1 & 0xFF:X2}");
                return null;
        }
    }
}