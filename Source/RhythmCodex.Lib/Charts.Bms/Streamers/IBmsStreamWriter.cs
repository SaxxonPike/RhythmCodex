using System.Collections.Generic;
using System.IO;
using RhythmCodex.Charts.Bms.Model;

namespace RhythmCodex.Charts.Bms.Streamers;

public interface IBmsStreamWriter
{
    void Write(Stream stream, IEnumerable<BmsCommand> commands);
}