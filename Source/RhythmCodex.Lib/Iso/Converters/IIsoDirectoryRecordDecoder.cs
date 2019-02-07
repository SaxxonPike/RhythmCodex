using System.IO;
using RhythmCodex.Iso.Model;

namespace RhythmCodex.Iso.Converters
{
    public interface IIsoDirectoryRecordDecoder
    {
        IsoDirectoryRecord Decode(Stream stream, bool recordOnly);
    }
}