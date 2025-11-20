using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Streamers;

public interface ITextStreamReader
{
    List<string> Read(Stream stream);
}