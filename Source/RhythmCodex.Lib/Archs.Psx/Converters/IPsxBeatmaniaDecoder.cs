using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Converters;

public interface IPsxBeatmaniaDecoder
{
    List<BmDataFile> DecodeBmData(Stream source, long length);
}