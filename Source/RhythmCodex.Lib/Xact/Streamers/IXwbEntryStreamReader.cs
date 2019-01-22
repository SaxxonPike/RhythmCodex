using System.IO;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    public interface IXwbEntryStreamReader
    {
        WaveBankEntry Read(Stream source);
    }
}