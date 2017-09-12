using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class DdrStandardPanelMapper : IPanelMapper
    {
        public IPanelMapping Map(int panel)
        {
            switch (panel)
            {
                case 0: return new PanelMapping { Panel = 0, Player = 0 };
                case 1: return new PanelMapping { Panel = 1, Player = 0 };
                case 2: return new PanelMapping { Panel = 2, Player = 0 };
                case 3: return new PanelMapping { Panel = 3, Player = 0 };
                case 4: return new PanelMapping { Panel = 0, Player = 1 };
                case 5: return new PanelMapping { Panel = 1, Player = 1 };
                case 6: return new PanelMapping { Panel = 2, Player = 1 };
                case 7: return new PanelMapping { Panel = 3, Player = 1 };
                default: return null;
            }
        }

        public int? Map(IPanelMapping mapping)
        {
            switch (mapping.Player)
            {
                case 0:
                    switch (mapping.Panel)
                    {
                        case 0: return 0;
                        case 1: return 1;
                        case 2: return 2;
                        case 3: return 3;
                        default: return null;
                    }
                case 1:
                    switch (mapping.Panel)
                    {
                        case 0: return 4;
                        case 1: return 5;
                        case 2: return 6;
                        case 3: return 7;
                        default: return null;
                    }
                default:
                    return null;
            }
        }
    }
}
