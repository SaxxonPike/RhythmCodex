using System.Collections.Generic;
using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Tim.Converters;

public interface ITimDecoder
{
    IList<IBitmap> Decode(Stream stream);
}