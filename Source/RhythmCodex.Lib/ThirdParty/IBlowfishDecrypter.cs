namespace RhythmCodex.ThirdParty;

public interface IBlowfishDecrypter
{
    byte[] Decrypt(byte[] data, string cipher);
}