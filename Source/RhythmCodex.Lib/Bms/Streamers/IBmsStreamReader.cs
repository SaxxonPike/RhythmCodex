using System.Collections.Generic;
using System.IO;
using RhythmCodex.Bms.Model;

namespace RhythmCodex.Bms.Streamers
{
    public interface IBmsStreamReader
    {
        IEnumerable<BmsCommand> Read(Stream stream);
    }
}