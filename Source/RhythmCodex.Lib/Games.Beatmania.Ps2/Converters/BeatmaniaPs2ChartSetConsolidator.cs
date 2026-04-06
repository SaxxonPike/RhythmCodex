using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

/// <inheritdoc />
[Service]
public sealed class BeatmaniaPs2ChartSetConsolidator : IBeatmaniaPs2ChartSetConsolidator
{
    /// <inheritdoc />
    public (List<Chart> Charts, List<Sound> Sounds) Consolidate(BeatmaniaPs2ChartSet chartSet)
    {
        var uniqueSets = chartSet.ChartMaps
            .Select(x => (x.Value.BgmId, x.Value.KeysoundId))
            .Distinct()
            .Index()
            .ToDictionary(x => x.Item, x => x.Index);

        var masterSoundList = new List<Sound>();

        foreach (var ((bgmId, keysoundId), sampleMapId) in uniqueSets)
        {
            masterSoundList.AddRange(chartSet.Keysounds[keysoundId].Select(k =>
            {
                var sound = k.Clone();
                sound[NumericData.SampleMap] = sampleMapId;
                return sound;
            }));

            var bgm = chartSet.Bgms[bgmId].Clone();
            bgm[NumericData.SampleMap] = sampleMapId;
            masterSoundList.Add(bgm);
        }

        var masterChartList = new List<Chart>();

        foreach (var (chartIndex, setChart) in chartSet.Charts)
        {
            var chart = setChart.Clone();
            chart[NumericData.SampleMap] = uniqueSets[chartSet.ChartMaps[chartIndex]];
            masterChartList.Add(chart);
        }

        return (masterChartList, masterSoundList);
    }
}