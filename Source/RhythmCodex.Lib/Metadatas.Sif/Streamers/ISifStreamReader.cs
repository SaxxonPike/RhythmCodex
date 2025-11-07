using System.IO;
using RhythmCodex.Metadatas.Sif.Models;

namespace RhythmCodex.Metadatas.Sif.Streamers;

public interface ISifStreamReader
{
    SifInfo Read(Stream stream, long length);
}