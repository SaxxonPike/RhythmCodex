using RhythmCodex.Graphics.Models;
using RhythmCodex.Graphics.Tcb.Models;

namespace RhythmCodex.Graphics.Tcb.Converters;

public interface ITcbImageDecoder
{
    Bitmap Decode(TcbImage image);
}