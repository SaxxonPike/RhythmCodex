using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.ThirdParty
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