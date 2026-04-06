using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Pc.Converters;
using RhythmCodex.Games.Beatmania.Pc.Streamers;
using RhythmCodex.Games.Beatmania.Ps2.Converters;
using RhythmCodex.Games.Beatmania.Ps2.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.Games.Beatmania;

/// <inheritdoc cref="IBeatmaniaService"/>
[Service]
public class BeatmaniaService(IServiceProvider services)
    : RhythmCodexServiceBase(services), IBeatmaniaService
{
    public List<Chart> ReadPcCharts(Stream stream, BigRational rate) =>
        Svc<IBeatmaniaPc1StreamReader>().Read(stream, stream.Length)
            .Select(chart => Svc<IBeatmaniaPc1ChartDecoder>().Decode(chart.Data, rate))
            .ToList();

    public List<Sound> ReadPcSounds(Stream stream) =>
        Svc<IBeatmaniaPcAudioStreamReader>().Read(stream, stream.Length)
            .Select(Svc<IBeatmaniaPcAudioDecoder>().Decode)
            .Where(sound => sound != null)
            .Select(sound => sound!)
            .ToList();

    public Chart ReadOldPs2Chart(Stream stream)
    {
        var reader = Svc<IBeatmaniaPs2OldChartStreamReader>();
        var decoder = Svc<IBeatmaniaPs2OldChartDecoder>();
        var converter = Svc<IBeatmaniaPs2ChartConverter>();

        return converter.Convert(decoder.Decode(reader.Read(stream, stream.Length).Span));
    }

    public Chart ReadNewPs2Chart(Stream stream)
    {
        var reader = Svc<IBeatmaniaPs2NewChartStreamReader>();
        var decoder = Svc<IBeatmaniaPs2NewChartDecoder>();
        var converter = Svc<IBeatmaniaPs2ChartConverter>();

        return converter.Convert(decoder.Decode(reader.Read(stream, stream.Length).Span));
    }
}