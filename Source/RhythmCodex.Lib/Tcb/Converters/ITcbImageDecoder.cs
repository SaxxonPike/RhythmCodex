using RhythmCodex.Graphics.Models;
using RhythmCodex.Tcb.Models;

namespace RhythmCodex.Tcb.Converters
{
    public interface ITcbImageDecoder
    {
        IBitmap Decode(TcbImage image);
    }
}