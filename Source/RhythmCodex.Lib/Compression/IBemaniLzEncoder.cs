namespace RhythmCodex.Compression
{
    public interface IBemaniLzEncoder
    {
        byte[] Encode(byte[] source);
    }
}