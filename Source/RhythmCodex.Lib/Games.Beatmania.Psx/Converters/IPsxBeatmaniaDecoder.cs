using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Djmain.Model;
using RhythmCodex.Charts.Models;
using RhythmCodex.Games.Beatmania.Psx.Models;

namespace RhythmCodex.Games.Beatmania.Psx.Converters;

public interface IPsxBeatmaniaDecoder
{
    Chart DecodeChart(Stream source, DjmainDecodeOptions options);
    List<PsxBeatmaniaFile> DecodeBmData(Stream source, long length);
    List<PsxBeatmaniaSysFile> DecodeSysData(Stream source, long length);
}