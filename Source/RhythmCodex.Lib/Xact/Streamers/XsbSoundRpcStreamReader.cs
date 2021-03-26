using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XsbSoundRpcStreamReader : IXsbSoundRpcStreamReader
    {
        public XsbSoundRpc Read(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}