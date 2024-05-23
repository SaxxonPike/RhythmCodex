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
public class BeatmaniaService(IServiceProvider services) : RhythmCodexServiceBase(services), IBeatmaniaService
{
    public List<Chart> ReadPcCharts(Stream stream, BigRational rate) =>
        Svc<IBeatmaniaPc1StreamReader>().Read(stream, stream.Length)
            .Select(chart => Svc<IBeatmaniaPc1ChartDecoder>().Decode(chart.Data, rate))
            .ToList();

    public List<Sound> ReadPcSounds(Stream stream) =>
        Svc<IBeatmaniaPcAudioStreamReader>().Read(stream, stream.Length)
            .Select(Svc<IBeatmaniaPcAudioDecoder>().Decode)
            .ToList();

    public Chart ReadOldPs2Chart(Stream stream) =>
        Svc<IBeatmaniaPs2ChartDecoder>()
            .Decode(Svc<IBeatmaniaPs2OldChartEventStreamReader>()
                .Read(stream, stream.Length));

    public Chart ReadNewPs2Chart(Stream stream) =>
        Svc<IBeatmaniaPs2ChartDecoder>()
            .Decode(Svc<IBeatmaniaPs2NewChartEventStreamReader>()
                .Read(stream, stream.Length));
}