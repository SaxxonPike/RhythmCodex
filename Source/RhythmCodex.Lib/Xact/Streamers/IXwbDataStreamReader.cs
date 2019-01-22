using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbDataStreamReader
    {
        WaveBankData Read(Stream source);
    }
}