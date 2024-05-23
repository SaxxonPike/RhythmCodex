using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Text.Streamers;

public interface ITextStreamReader
{
    IList<string> Read(Stream stream);
}