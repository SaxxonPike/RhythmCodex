namespace RhythmCodex.Tim.Converters
{
    public interface ITimDataDecoder
    {
        int[] Decode4Bit(byte[] data, int stride, int height);
        int[] Decode8Bit(byte[] data, int stride, int height);
        int[] Decode16Bit(byte[] data, int stride, int height);
        int[] Decode24Bit(byte[] data, int stride, int height);
    }
}