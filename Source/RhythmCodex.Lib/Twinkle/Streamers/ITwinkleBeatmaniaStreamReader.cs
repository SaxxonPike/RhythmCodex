using System.Collections.Generic;
using System.IO;
using RhythmCodex.Twinkle.Model;

namespace RhythmCodex.Twinkle.Streamers;

public interface ITwinkleBeatmaniaStreamReader
{
    IEnumerable<TwinkleBeatmaniaChunk> Read(Stream stream, long length, bool skipHeader = false);
}