namespace RhythmCodex.Compression
{
    public interface IZlibToGzipConverter
    {
        byte[] Convert(byte[] zlibData);
    }
}