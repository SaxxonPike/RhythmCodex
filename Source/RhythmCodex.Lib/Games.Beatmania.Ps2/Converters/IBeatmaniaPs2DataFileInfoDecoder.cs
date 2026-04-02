using System;
using System.Collections.Generic;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2DataFileInfoDecoder
{
    List<BeatmaniaPs2DataFileInfo> Decode(ReadOnlySpan<byte> data, int dataFileInfoOffset, BeatmaniaPs2FormatType type);
}