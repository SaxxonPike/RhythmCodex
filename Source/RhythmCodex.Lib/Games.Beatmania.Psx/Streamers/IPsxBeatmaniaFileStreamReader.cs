using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Psx.Models;

namespace RhythmCodex.Games.Beatmania.Psx.Streamers;

public interface IPsxBeatmaniaFileStreamReader
{
    List<PsxBeatmaniaFileEntry> ReadDirectory(Stream stream);
    IEnumerable<ReadOnlyMemory<byte>> ReadEntries(Stream stream, IEnumerable<PsxBeatmaniaFileEntry> entries);
}