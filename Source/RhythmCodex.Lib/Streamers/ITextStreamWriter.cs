using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Streamers;

public interface ITextStreamWriter
{
    void Write(Stream stream, IEnumerable<string> lines);
}