namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573AudioDecrypter
    {
        byte[] DecryptNew(byte[] input, int key0, int key1, int key2);
        byte[] DecryptOld(byte[] input, int key);
    }
}