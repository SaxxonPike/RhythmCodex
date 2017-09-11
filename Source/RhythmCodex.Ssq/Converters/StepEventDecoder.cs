using System.Collections.Generic;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
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
                var metricOffset = (BigRational) step.MetricOffset / SsqConstants.MeasureLength;

                if (panels == 0x00)
                {
                    freeze = true;
                    panels = step.ExtraPanels ?? 0;
                }

                if ((panels & 0x0F) == 0x0F)
                {
                    yield return new Event
                    {
                        [NumericData.MetricOffset] = metricOffset,
                        [NumericData.Player] = 1,
                        [FlagData.Shock] = true
                    };
                    panels &= 0xF0;
                }

                if ((panels & 0xF0) == 0xF0)
                {
                    yield return new Event
                    {
                        [NumericData.MetricOffset] = metricOffset,
                        [NumericData.Player] = 2,
                        [FlagData.Shock] = true
                    };
                    panels &= 0x0F;
                }

                var panelNumber = 0;
                while (panels > 0)
                {
                    if ((panels & 1) != 0)
                    {
                        var mappedPanel = _panelMapper.Map(panelNumber);
                        var isMapped = mappedPanel != null;

                        yield return new Event
                        {
                            [NumericData.MetricOffset] = metricOffset,
                            [NumericData.SourceColumn] = panelNumber,
                            [NumericData.Column] = isMapped ? mappedPanel.Panel : (BigRational?)null,
                            [NumericData.Player] = isMapped ? mappedPanel.Player : (BigRational?)null,
                            [FlagData.Freeze] = freeze ? true : (bool?)null,
                            [FlagData.Note] = freeze ? (bool?)null : true
                        };
                    }

                    panels >>= 1;
                    panelNumber++;
                }
            }
        }
    }
}
