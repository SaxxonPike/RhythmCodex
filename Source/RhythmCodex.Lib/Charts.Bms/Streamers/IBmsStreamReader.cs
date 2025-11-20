using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charts.Bms.Model;

namespace RhythmCodex.Charts.Bms.Streamers;

public interface IBmsStreamReader
{
    IEnumerable<BmsCommand> Read(Stream stream);
}