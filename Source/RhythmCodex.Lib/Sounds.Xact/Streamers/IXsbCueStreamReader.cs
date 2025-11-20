using System.Collections.Generic;
using System.IO;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

public interface IXsbCueStreamReader
{
    IEnumerable<XsbCue> ReadSimple(Stream stream, int count);
    IEnumerable<XsbCue> ReadComplex(Stream stream, int count);
}