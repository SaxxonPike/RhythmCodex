using System.IO;
using RhythmCodex.Sounds.Xa.Models;

namespace RhythmCodex.Sounds.Xa.Streamers;

public interface IXaStreamReader
{
    XaChunk Read(Stream stream, int channels, int interleave);
}