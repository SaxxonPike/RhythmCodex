using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.Xa.Streamers
{
    [Service]
    public class XaStreamReader : IXaStreamReader
    {
        public XaChunk Read(Stream stream, int channels, int interleave)
        {
            throw new System.NotImplementedException();
        }
    }
}