using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XsbSoundDspStreamReader : IXsbSoundDspStreamReader
    {
        public XsbSoundDsp Read(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}