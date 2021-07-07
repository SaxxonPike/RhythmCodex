using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class StepEventEncoder : IStepEventEncoder
    {
        public IList<Step> Encode(IEnumerable<Event> events, IPanelMapper panelMapper)
        {
            var eventList = events
                .Where(ev => ev[FlagData.Note] == true)
                .GroupBy(ev => ev[NumericData.MetricOffset])
                .OrderBy(g => g.Key).ToList();

            return eventList
                .SelectMany(g =>
                {
                    var steps = new List<Step>();
                    var panelValue = 0;

                    foreach (var panel in g)
                    {
                        var outPanel = panelMapper.Map(new PanelMapping
                        {
                            Panel = (int) (panel[NumericData.Column] ?? BigRational.Zero),
                            Player = (int) (panel[NumericData.Player] ?? BigRational.Zero)
                        });

                        if (outPanel == null)
                            continue;

                        if (panel[FlagData.Freeze] == true)
                        {
                            steps.Add(new Step
                            {
                                MetricOffset = (int) (g.Key * SsqConstants.MeasureLength),
                                Panels = 0x00,
                                ExtraPanels = unchecked((byte) outPanel.Value),
                                ExtraPanelInfo = 0x01
                            });
                            continue;
                        }

                        panelValue |= 1 << outPanel.Value;
                    }

                    if (panelValue != 0)
                    {
                        steps.Add(new Step
                        {
                            MetricOffset = (int)(g.Key * SsqConstants.MeasureLength),
                            Panels = unchecked((byte) panelValue),
                            ExtraPanels = null,
                            ExtraPanelInfo = null
                        });
                    }

                    return steps;
                }).ToList();
        }
    }
}