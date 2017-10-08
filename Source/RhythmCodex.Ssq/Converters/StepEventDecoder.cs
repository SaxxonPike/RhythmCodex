﻿using System.Collections.Generic;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Ssq.Mappers;
using RhythmCodex.Ssq.Model;

namespace RhythmCodex.Ssq.Converters
{
    [Service]
    public class StepEventDecoder : IStepEventDecoder
    {
        private readonly IStepPanelSplitter _stepPanelSplitter;

        public StepEventDecoder(IStepPanelSplitter stepPanelSplitter)
        {
            _stepPanelSplitter = stepPanelSplitter;
        }

        public IEnumerable<IEvent> Decode(IEnumerable<Step> steps, IPanelMapper panelMapper)
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

                foreach (var panelNumber in _stepPanelSplitter.Split(panels))
                {
                    var mappedPanel = panelMapper.Map(panelNumber);
                    var isMapped = mappedPanel != null;

                    yield return new Event
                    {
                        [NumericData.MetricOffset] = metricOffset,
                        [NumericData.SourceColumn] = panelNumber,
                        [NumericData.Column] = isMapped ? mappedPanel.Panel : (BigRational?) null,
                        [NumericData.Player] = isMapped ? mappedPanel.Player : (BigRational?) null,
                        [FlagData.Freeze] = freeze ? true : (bool?) null,
                        [FlagData.Note] = freeze ? (bool?) null : true
                    };
                }
            }
        }
    }
}