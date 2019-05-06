namespace RhythmCodex.ThirdParty
{
    public interface IBlowfishDecrypter
    {
        byte[] DecryptCts(byte[] data, string cipher);
    }
}