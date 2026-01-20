using System;
using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Charts.Bmson.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.IoC;
using RhythmCodex.Metadatas.Models;

namespace RhythmCodex.Charts.Bmson.Converters;

[Service]
public class BmsonChartConverter : IBmsonChartConverter
{
    public (Chart Chart, Dictionary<int, string> SoundFileMap) Import(BmsonFile bmson)
    {
        throw new NotImplementedException();
    }

    public BmsonFile Export(Chart chart, Dictionary<int, string> soundFileMap)
    {
        var bpm = chart.Events
            .Where(x => x[NumericData.Bpm] != null)
            .OrderBy(x => x[NumericData.MetricOffset])
            .Select(x => x[NumericData.Bpm])
            .FirstOrDefault() ?? chart[NumericData.Bpm] ?? 120;
        
        var result = new BmsonFile
        {
            Version = "1.0.0",
            Info =
            {
                Title = chart[StringData.Title],
                SubTitle = chart[StringData.Subtitle],
                Artist =  chart[StringData.Artist],
                SubArtists = chart[StringData.Subartist] is {} subArtist ? [subArtist] : null,
                Genre =  chart[StringData.Genre],
                ModeHint = chart[StringData.Type],
                ChartName = chart[StringData.Description],
                Level = (long?)chart[NumericData.PlayLevel] ?? 0,
                InitBpm = (double)bpm,
                Resolution = 420
            }
        };

        throw new NotImplementedException();
    }
}