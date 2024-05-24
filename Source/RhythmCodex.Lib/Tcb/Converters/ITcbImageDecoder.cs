using RhythmCodex.Graphics.Models;
using RhythmCodex.Tcb.Models;

namespace RhythmCodex.Tcb.Converters;

public interface ITcbImageDecoder
{
    Bitmap Decode(TcbImage image);
}