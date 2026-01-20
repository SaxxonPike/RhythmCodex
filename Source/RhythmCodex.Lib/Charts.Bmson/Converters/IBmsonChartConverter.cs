using System.Collections.Generic;
using RhythmCodex.Charts.Bmson.Model;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Charts.Bmson.Converters;

public interface IBmsonChartConverter
{
    (Chart Chart, Dictionary<int, string> SoundFileMap) Import(BmsonFile bmson);
    BmsonFile Export(Chart chart, Dictionary<int, string> soundFileMap);
}