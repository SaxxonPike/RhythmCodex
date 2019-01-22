using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbHeaderStreamReader
    {
        WaveBankHeader Read(Stream source);
    }
}