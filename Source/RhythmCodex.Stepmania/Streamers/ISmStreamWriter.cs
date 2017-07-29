using System.Collections.Generic;
using System.IO;
using RhythmCodex.Stepmania.Model;

namespace RhythmCodex.Stepmania.Streamers
{
    public interface ISmStreamWriter
    {
        void Write(Stream stream, IEnumerable<Command> commands);
    }
}