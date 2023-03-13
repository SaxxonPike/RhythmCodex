using System.Collections.Generic;
using System.IO;
using RhythmCodex.Bms.Model;

namespace RhythmCodex.Bms.Streamers;

public interface IBmsStreamWriter
{
    void Write(Stream stream, IEnumerable<BmsCommand> commands);
}