using System.IO;
using RhythmCodex.FileSystems.Iso.Model;

namespace RhythmCodex.FileSystems.Iso.Converters;

public interface IIsoDirectoryRecordDecoder
{
    IsoDirectoryRecord? Decode(Stream stream, bool recordOnly);
}