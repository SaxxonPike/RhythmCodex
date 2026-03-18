using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Streamers;

public interface IBmDataStreamReader
{
    List<BmDataPakEntry> ReadDirectory(Stream stream);
    IEnumerable<ReadOnlyMemory<byte>> ReadEntries(Stream stream, IEnumerable<BmDataPakEntry> entries);
}