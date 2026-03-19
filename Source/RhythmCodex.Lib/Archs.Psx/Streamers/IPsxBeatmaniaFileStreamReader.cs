using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Streamers;

public interface IPsxBeatmaniaFileStreamReader
{
    List<PsxBeatmaniaFileEntry> ReadDirectory(Stream stream);
    IEnumerable<ReadOnlyMemory<byte>> ReadEntries(Stream stream, IEnumerable<PsxBeatmaniaFileEntry> entries);
}