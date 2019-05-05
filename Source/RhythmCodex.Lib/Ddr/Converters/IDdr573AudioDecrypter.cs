namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573AudioDecrypter
    {
        byte[] Decrypt(byte[] input, byte[] key, byte[] scramble, int counter);
    }
}