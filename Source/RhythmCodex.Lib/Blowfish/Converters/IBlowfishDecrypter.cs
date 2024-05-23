namespace RhythmCodex.Blowfish.Converters;

public interface IBlowfishDecrypter
{
    byte[] Decrypt(byte[] data, string cipher);
}