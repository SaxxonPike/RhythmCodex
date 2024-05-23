using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Xact.Streamers;

public interface IXsbCueNameTableStreamReader
{
    IEnumerable<string> Read(Stream stream, int length);
}