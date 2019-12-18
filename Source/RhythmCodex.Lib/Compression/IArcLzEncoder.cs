namespace RhythmCodex.Compression
{
    public interface IArcLzEncoder
    {
        byte[] Encode(byte[] source);
    }
}