using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Ssq.Model;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Step1.Converters;

[Service]
public class Step1ChartInfoDecoder : IStep1ChartInfoDecoder
{
    private readonly ILogger _logger;

    public Step1ChartInfoDecoder(ILogger logger)
    {
        _logger = logger;
    }
        
    public ChartInfo Decode(int metadata, int playerCount, int panelCount)
    {
        return new ChartInfo
        {
            Difficulty = GetDifficulty(metadata),
            PanelCount = panelCount,
            PlayerCount = playerCount,
            Type = GetType(metadata, playerCount, panelCount)
        };
    }

    private string GetType(int metadata, int playerCount, int panelCount)
    {
        if (playerCount == 1)
        {
            switch (panelCount)
            {
                case 3: return SmGameTypes.ThreePanel;
                case 4: return SmGameTypes.Single;
                case 6: return SmGameTypes.Solo;
                case 8: return SmGameTypes.Double;
            }
        }

        if (playerCount == 2)
        {
            switch (metadata & 0xFF)
            {
                case 0x01: return SmGameTypes.Couple;
                case 0x02: return SmGameTypes.Double;
            }                
        }
            
        _logger.Warning($"Unrecognized chart type {metadata & 0xFF:X2}");
        return null;
    }

    private string GetDifficulty(int metadata)
    {
        switch ((metadata >> 8) & 0xFF)
        {
            case 0x00:
                return SmNotesDifficulties.Easy;
            case 0x01:
                return SmNotesDifficulties.Medium;
            case 0x02:
                return SmNotesDifficulties.Hard;
            default:
                _logger.Warning($"Unrecognized chart difficulty {(metadata >> 8) & 0xFF:X2}");
                return "Edit";
        }
    }
}