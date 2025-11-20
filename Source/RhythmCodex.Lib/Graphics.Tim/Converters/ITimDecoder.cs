using System.Collections.Generic;
using System.IO;
using RhythmCodex.Graphics.Models;

namespace RhythmCodex.Graphics.Tim.Converters;

public interface ITimDecoder
{
    List<Bitmap> Decode(Stream stream);
}