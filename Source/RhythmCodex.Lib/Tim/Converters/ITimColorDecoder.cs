namespace RhythmCodex.Tim.Converters
{
    public interface ITimColorDecoder
    {
        int Decode16Bit(int color);
        int Decode24Bit(int color);
    }
}