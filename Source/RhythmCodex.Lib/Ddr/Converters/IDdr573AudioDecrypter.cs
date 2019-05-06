namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573AudioDecrypter
    {
        byte[] Decrypt(byte[] input, int key0, int key1, int key2);
    }
}