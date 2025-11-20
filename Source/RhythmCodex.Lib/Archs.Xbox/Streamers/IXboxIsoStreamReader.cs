using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Xbox.Model;

namespace RhythmCodex.Archs.Xbox.Streamers;

public interface IXboxIsoStreamReader
{
    IEnumerable<XboxIsoFileEntry> Read(Stream stream, long length);
    Memory<byte> Extract(Stream stream, XboxIsoFileEntry entry);
}