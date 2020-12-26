using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Plugin.BouncyCastle.Lib.crypto.engines;
using RhythmCodex.Plugin.BouncyCastle.Lib.crypto.modes;
using RhythmCodex.Plugin.BouncyCastle.Lib.crypto.parameters;
using RhythmCodex.ThirdParty;

namespace RhythmCodex.Plugin.BouncyCastle
{
    [Service]
    public class CryptoProvider : ICryptoProvider
    {
        public byte[] DecryptCtsCbcBlowfish(byte[] data, string key)
        {
            //var paddedData = data.AsSpan().PadUpTo(data.Length + (8 - data.Length % 8));
            var paddedData = data;
            var keyBytes = Encodings.ASCII.GetBytes(key);
            var iv = new byte[8];
            var cipher = new CtsBlockCipher(new CbcBlockCipher(new BlowfishEngine()));
            var keyParam = new ParametersWithIV(new KeyParameter(keyBytes), iv);
            cipher.Init(false, keyParam);
            return cipher.DoFinal(paddedData, 0, data.Length);
        }
    }
}