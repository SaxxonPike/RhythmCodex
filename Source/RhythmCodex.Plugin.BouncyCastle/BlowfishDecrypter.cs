using System;
using System.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using RhythmCodex.Encryptions.Blowfish.Converters;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;

namespace RhythmCodex.Plugin.BouncyCastle;

[Service]
public class BlowfishDecrypter : IBlowfishDecrypter
{
    public Memory<byte> Decrypt(ReadOnlySpan<byte> data, byte[] key, byte padByte)
    {
        //
        // Prepare the cipher. (The IV is blank for supported games that use this encryption method.)
        //

        var keyBytes = key.AsSpan().ToArray();
        var iv = new byte[8];
        var cipher = CipherUtilities.GetCipher("BLOWFISH/CBC/WITHCTS");
        cipher.Init(false, new ParametersWithIV(new KeyParameter(keyBytes), iv));

        //
        // Pad the input buffer if necessary.
        //

        var pad = (data.Length & 7) == 0 ? 8 : 0;
        var output = new byte[data.Length];
        var outputOffset = 0;
        var inputOffset = 0;

        //
        // BouncyCastle only operates in big-endian mode, so we have to insert the data swapped.
        //

        Span<byte> buffer = stackalloc byte[16];

        for (inputOffset = 0; inputOffset < data.Length - 3; inputOffset += 4)
        {
            for (var j = 0; j < 4; j++)
                Send(data[inputOffset + (j ^ 3)], buffer);
        }

        for (; inputOffset < data.Length; inputOffset++)
        {
            
        }
        
        Finish(buffer);
        return output;

        //
        // Helper routines.
        //

        void Finish(Span<byte> buffer)
        {
            for (var i = 0; i < pad; i++)
                Send(0, buffer);

            var len = cipher.DoFinal(buffer);
            if (len > 0)
                Commit(buffer, len);
        }

        void Send(byte input, Span<byte> buffer)
        {
            var len = cipher.ProcessByte(input, buffer);
            if (len > 0)
                Commit(buffer, len);
        }

        void Commit(Span<byte> buffer, int len)
        {
            for (var j = 0; j < len; j++)
            {
                if (outputOffset < output.Length)
                    output[outputOffset++] = buffer[j ^ 3];
            }
        }
    }
}