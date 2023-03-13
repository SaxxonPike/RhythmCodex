namespace RhythmCodex.ThirdParty;

public interface ICryptoProvider
{
    byte[] DecryptCtsCbcBlowfish(byte[] data, string cipher);
}