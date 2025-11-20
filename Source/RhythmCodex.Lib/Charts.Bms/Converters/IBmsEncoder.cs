using System.Collections.Generic;
using RhythmCodex.Charts.Bms.Model;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Charts.Bms.Converters;

public interface IBmsEncoder
{
    List<BmsCommand> Encode(Chart chart, BmsEncoderOptions? options = null);
}