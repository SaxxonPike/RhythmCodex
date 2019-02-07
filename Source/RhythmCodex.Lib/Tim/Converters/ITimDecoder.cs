using System.Collections.Generic;
using System.IO;
using RhythmCodex.Graphics.Models;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.Tim.Converters
{
    public interface ITimDecoder
    {
        IList<RawBitmap> Decode(Stream stream);
    }
}