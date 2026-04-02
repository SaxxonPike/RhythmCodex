using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2Decoder
{
    List<BeatmaniaPs2ChartSet> Decode(Func<string, Stream> openFile,
        BeatmaniaPs2FormatType type);
}