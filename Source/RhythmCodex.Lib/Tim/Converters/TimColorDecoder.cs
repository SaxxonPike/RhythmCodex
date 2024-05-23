using RhythmCodex.IoC;

namespace RhythmCodex.Tim.Converters;

[Service]
public class TimColorDecoder : ITimColorDecoder
{
    public int Decode16Bit(int color)
    {
        var red = (color & 0x001F) << 19;
        var green = (color & 0x03E0) << 6;
        var blue = (color & 0x7C00) >> 7;
        var mask = (color & 0x7F8000) << 9;
        return red | green | blue | mask;
    }

    public int Decode24Bit(int color)
    {
        var red = (color & 0x0000FF) << 16;
        var green = (color & 0x00FF00);
        var blue = (color & 0xFF0000) >> 16;
        return red | green | blue | ~0xFFFFFF;
    }
}