using System.Collections.Generic;
using RhythmCodex.Cd.Model;

namespace RhythmCodex.Iso.Converters;

public interface IIsoCdFileDecoder
{
    IList<ICdFile> Decode(IEnumerable<ICdSector> cdSectors);
}