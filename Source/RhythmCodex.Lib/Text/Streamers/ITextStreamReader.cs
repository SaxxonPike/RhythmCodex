using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Text.Streamers;

public interface ITextStreamReader
{
    List<string> Read(Stream stream);
}