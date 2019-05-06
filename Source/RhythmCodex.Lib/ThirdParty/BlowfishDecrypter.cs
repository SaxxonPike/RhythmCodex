using System.Security.Cryptography;
using RhythmCodex.IoC;
using RhythmCodex.Thirdparty.Blowfish;

namespace RhythmCodex.ThirdParty
{
    [Service]
    public class BlowfishDecrypter : IBlowfishDecrypter
    {
        public byte[] DecryptCts(byte[] data, string cipher)
        {
            var bf = new BlowFish(cipher);
            return bf.Decrypt(data, CipherMode.CTS);
        }
    }
}