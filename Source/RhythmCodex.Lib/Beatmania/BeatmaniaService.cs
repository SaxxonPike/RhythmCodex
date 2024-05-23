using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Beatmania.Converters;
using RhythmCodex.Beatmania.Streamers;
using RhythmCodex.Charting.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Beatmania;

/// <inheritdoc cref="IBeatmaniaService"/>
[Service]
public class BeatmaniaService : RhythmCodexServiceBase, IBeatmaniaService
{
    public BeatmaniaService(IServiceProvider services)
        : base(services)
    {
    }

    public List<IChart> ReadPcCharts(Stream stream, BigRational rate) =>
        Svc<IBeatmaniaPc1StreamReader>().Read(stream, stream.Length)
            .Select(chart => Svc<IBeatmaniaPc1ChartDecoder>().Decode(chart.Data, rate))
            .ToList();

    public List<ISound> ReadPcSounds(Stream stream) =>
        Svc<IBeatmaniaPcAudioStreamReader>().Read(stream, stream.Length)
            .Select(Svc<IBeatmaniaPcAudioDecoder>().Decode)
            .ToList();

    public IChart ReadOldPs2Chart(Stream stream) =>
        Svc<IBeatmaniaPs2ChartDecoder>()
            .Decode(Svc<IBeatmaniaPs2OldChartEventStreamReader>()
                .Read(stream, stream.Length));

    public IChart ReadNewPs2Chart(Stream stream) =>
        Svc<IBeatmaniaPs2ChartDecoder>()
            .Decode(Svc<IBeatmaniaPs2NewChartEventStreamReader>()
                .Read(stream, stream.Length));
}