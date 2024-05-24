using System;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using RhythmCodex.Blowfish.Converters;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Plugin.BouncyCastle;

[Service]
public class BlowfishDecrypter : IBlowfishDecrypter
{
    public Memory<byte> Decrypt(ReadOnlySpan<byte> data, string key)
    {
        var paddedData = data;
        var keyBytes = Encodings.ASCII.GetBytes(key);
        var iv = new byte[8];
        var cipher = new CtsBlockCipher(new CbcBlockCipher(new BlowfishEngine()));
        var keyParam = new ParametersWithIV(new KeyParameter(keyBytes), iv);
        cipher.Init(false, keyParam);
        return cipher.DoFinal(paddedData.ToArray(), 0, data.Length);
    }
}