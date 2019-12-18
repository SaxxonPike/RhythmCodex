using System.Collections.Generic;
using System.IO;
using RhythmCodex.Arc.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Arc.Streamers
{
    [Service]
    public class ArcStreamWriter : IArcStreamWriter
    {
        public void Write(Stream target, IEnumerable<ArcFile> charts)
        {
            throw new System.NotImplementedException();
        }
    }
}