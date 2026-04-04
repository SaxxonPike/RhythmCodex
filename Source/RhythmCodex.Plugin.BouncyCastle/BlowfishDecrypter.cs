using System;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using RhythmCodex.Encryptions.Blowfish.Converters;
using RhythmCodex.IoC;

namespace RhythmCodex.Plugin.BouncyCastle;

[Service]
public sealed class BlowfishDecrypter : IBlowfishDecrypter
{
    public Memory<byte> Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
    {
        //
        // Prepare the cipher. (The IV is blank for supported games that use this encryption method.)
        // BouncyCastle's stock configuration only supports big endian, so we use a fork
        // of the code that uses little endian instead.
        //

        var keyBytes = key.ToArray();
        var iv = new byte[8];
        var cipher = new CtsBlockCipher(new CbcBlockCipher(new BlowfishEngine()));
        cipher.Init(false, new ParametersWithIV(new KeyParameter(keyBytes), iv));

        //
        // Pad the input buffer if necessary.
        //

        var pad = (data.Length & 7) == 0 ? 8 : 0;
        var input = new byte[data.Length + pad];
        data.CopyTo(input);
        var output = new byte[input.Length];
        cipher.DoFinal(input, output);

        return output;
    }
}