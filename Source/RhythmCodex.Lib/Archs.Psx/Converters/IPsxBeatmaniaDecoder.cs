using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.Charts.Models;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxBeatmaniaDecoder
{
    Chart DecodeChart(Stream source);
    List<PsxBeatmaniaFile> DecodeBmData(Stream source, long length);
    List<PsxBeatmaniaSysFile> DecodeSysData(Stream source, long length);
}