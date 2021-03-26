using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXsbSoundRpcStreamReader
    {
        XsbSoundRpc Read(Stream stream);
    }
}