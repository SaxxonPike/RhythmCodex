using System.IO;
using RhythmCodex.Archs.Psx.Model;

namespace RhythmCodex.Archs.Psx.Streamers;

public interface IBmDataKeysoundBlockReader
{
    BmDataKeysoundBlock Read(Stream stream);
}