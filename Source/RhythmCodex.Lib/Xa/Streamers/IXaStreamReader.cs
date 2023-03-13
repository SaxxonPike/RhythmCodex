using System.IO;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.Xa.Streamers;

public interface IXaStreamReader
{
    XaChunk Read(Stream stream, int channels, int interleave);
}