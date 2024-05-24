using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Streamers;

public interface IXboxIsoStreamReader
{
    IEnumerable<XboxIsoFileEntry> Read(Stream stream, long length);
    Memory<byte> Extract(Stream stream, XboxIsoFileEntry entry);
}