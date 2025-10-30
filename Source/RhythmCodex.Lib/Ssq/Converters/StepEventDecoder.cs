using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters;

[Service]
public class StepEventDecoder(IStepPanelSplitter stepPanelSplitter) : IStepEventDecoder
{
    public List<Event> Decode(IEnumerable<Step> steps, IPanelMapper panelMapper)
    {
        return Do().ToList();

        IEnumerable<Event> Do()
        {
            if (panelMapper == null)
                throw new RhythmCodexException("Panel mapper cannot be null");

            var stepList = steps.AsCollection();

            foreach (var step in stepList)
            {
                var panels = step.Panels;
                var freeze = false;
                var metricOffset = (BigRational)step.MetricOffset / SsqConstants.MeasureLength;

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
                        [NumericData.Player] = 0,
                        [FlagData.Shock] = true
                    };
                    panels &= 0xF0;
                }

                if ((panels & 0xF0) == 0xF0)
                {
                    yield return new Event
                    {
                        [NumericData.MetricOffset] = metricOffset,
                        [NumericData.Player] = 1,
                        [FlagData.Shock] = true
                    };
                    panels &= 0x0F;
                }

                foreach (var panelNumber in stepPanelSplitter.Split(panels))
                {
                    var mappedPanel = panelMapper.Map(panelNumber);
                    var isMapped = mappedPanel != null;

                    yield return new Event
                    {
                        [NumericData.MetricOffset] = metricOffset,
                        [NumericData.SourceColumn] = panelNumber,
                        [NumericData.Column] = isMapped ? mappedPanel.Panel : null,
                        [NumericData.Player] = isMapped ? mappedPanel.Player : null,
                        [FlagData.Freeze] = freeze ? true : null,
                        [FlagData.Note] = freeze ? null : true
                    };
                }
            }
        }
    }
}