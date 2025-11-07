using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXsbSoundRpcStreamReader
{
    XsbSoundRpc Read(Stream stream);
}