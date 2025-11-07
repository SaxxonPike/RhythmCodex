using System.Collections.Generic;
using System.IO;
using RhythmCodex.Archs.Twinkle.Model;

namespace RhythmCodex.Archs.Twinkle.Streamers;

public interface ITwinkleBeatmaniaStreamReader
{
    IEnumerable<TwinkleBeatmaniaChunk> Read(Stream stream, long length, bool skipHeader = false);
}