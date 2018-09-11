using System.Collections.Generic;
using System.IO;

namespace RhythmCodex.Text.Streamers
{
    public interface ITextStreamWriter
    {
        void Write(Stream stream, IEnumerable<string> lines);
    }
}