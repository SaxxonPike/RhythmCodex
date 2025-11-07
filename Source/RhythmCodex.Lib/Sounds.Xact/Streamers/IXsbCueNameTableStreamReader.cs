using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXsbCueNameTableStreamReader
{
    IEnumerable<string> Read(Stream stream, int length);
}