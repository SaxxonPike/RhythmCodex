using System.Collections.Generic;
using RhythmCodex.Charts.Bms.Converters;
using RhythmCodex.Charts.Bmson.Model;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Charts.Bmson.Converters;

public interface IBmsonChartConverter
{
    BmsonFile Export(
        Chart chart,
        BmsonEncoderOptions options);
}