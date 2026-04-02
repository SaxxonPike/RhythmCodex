using System;
using System.Collections.Generic;
using RhythmCodex.Games.Beatmania.Ps2.Models;

namespace RhythmCodex.Games.Beatmania.Ps2.Converters;

public interface IBeatmaniaPs2SongInfoDecoder
{
    List<BeatmaniaPs2SongInfo> Decode(ReadOnlySpan<byte> data, int songInfoOffset, BeatmaniaPs2FormatType type);
}