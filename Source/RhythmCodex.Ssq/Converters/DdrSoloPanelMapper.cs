using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class DdrSoloPanelMapper : IPanelMapper
    {
        public IPanelMapping Map(int panel)
        {
            switch (panel)
            {
                case 0: return new PanelMapping { Panel = 0, Player = 0 };
                case 1: return new PanelMapping { Panel = 2, Player = 0 };
                case 2: return new PanelMapping { Panel = 3, Player = 0 };
                case 3: return new PanelMapping { Panel = 5, Player = 0 };
                case 4: return new PanelMapping { Panel = 1, Player = 0 };
                case 6: return new PanelMapping { Panel = 4, Player = 0 };
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
                        case 1: return 4;
                        case 2: return 1;
                        case 3: return 2;
                        case 4: return 6;
                        case 5: return 3;
                        default: return null;
                    }
                default:
                    return null;
            }
        }
    }
}
