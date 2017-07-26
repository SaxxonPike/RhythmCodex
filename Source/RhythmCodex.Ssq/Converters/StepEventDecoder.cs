using System.Collections.Generic;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    public class StepEventDecoder : IStepEventDecoder
    {
        private readonly IPanelMapper _panelMapper;

        public StepEventDecoder(IPanelMapper panelMapper)
        {
            _panelMapper = panelMapper;
        }

        public IEnumerable<IEvent> Decode(IEnumerable<Step> steps)
        {
            var stepList = steps.AsList();
            
            foreach (var step in stepList)
            {
                var panels = step.Panels;
                var freeze = false;
                var shock = false;

                switch (panels)
                {
                    case 0x00:
                        freeze = true;
                        panels = step.ExtraPanels ?? 0;
                        break;
                    case 0xFF:
                        shock = true;
                        panels = 1;
                        break;
                }

                var panelNumber = 0;

                while (panels > 0)
                {
                    if ((panels & 1) != 0)
                    {
                        var mappedPanel = _panelMapper.Map(panelNumber);
                        var isMapped = mappedPanel.HasValue;

                        var ev = new Event
                        {
                            [NumericData.MetricOffset] = step.MetricOffset,
                            [NumericData.SourcePanel] = panelNumber
                        };

                        if (freeze)
                            ev[FlagData.Freeze] = true;

                        if (shock)
                            ev[FlagData.Shock] = true;
                        else if (isMapped)
                            ev[NumericData.Panel] = mappedPanel.Value.Panel;

                        if (isMapped)
                            ev[NumericData.Player] = mappedPanel.Value.Player;

                        yield return ev;
                    }

                    panels >>= 1;
                    panelNumber++;
                }
            }
        }
    }
}
