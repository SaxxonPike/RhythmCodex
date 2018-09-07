using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Bms.Streamers
{
    public interface IBmsStreamWriter
    {
        void Write(Stream stream, IEnumerable<string> commands);
    }
}