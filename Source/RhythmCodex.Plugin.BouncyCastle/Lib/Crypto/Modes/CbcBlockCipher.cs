using System;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

// ReSharper disable CheckNamespace

namespace Org.BouncyCastle.Crypto.Modes;

/**
* implements Cipher-Block-Chaining (CBC) mode on top of a simple cipher.
*/
public sealed class CbcBlockCipher
    : IBlockCipherMode
{
    private byte[] IV, cbcV, cbcNextV;
    private int blockSize;
    private bool encrypting;

    /**
    * Basic constructor.
    *
    * @param cipher the block cipher to be used as the basis of chaining.
    */
    public CbcBlockCipher(
        IBlockCipher cipher)
    {
        UnderlyingCipher = cipher;
        blockSize = cipher.GetBlockSize();

        IV = new byte[blockSize];
        cbcV = new byte[blockSize];
        cbcNextV = new byte[blockSize];
    }

    /**
    * return the underlying block cipher that we are wrapping.
    *
    * @return the underlying block cipher that we are wrapping.
    */
    public IBlockCipher UnderlyingCipher { get; }

    /**
    * Initialise the cipher and, possibly, the initialisation vector (IV).
    * If an IV isn't passed as part of the parameter, the IV will be all zeros.
    *
    * @param forEncryption if true the cipher is initialised for
    *  encryption, if false for decryption.
    * @param param the key and other data required by the cipher.
    * @exception ArgumentException if the parameters argument is
    * inappropriate.
    */
    public void Init(bool forEncryption, ICipherParameters parameters)
    {
        var oldEncrypting = encrypting;

        encrypting = forEncryption;

        if (parameters is ParametersWithIV ivParam)
        {
            if (ivParam.IVLength != blockSize)
                throw new ArgumentException("initialisation vector must be the same length as block size");

            ivParam.CopyIVTo(IV, 0, blockSize);

            parameters = ivParam.Parameters;
        }
        else
        {
            Arrays.Fill(IV, 0x00);
        }

        Reset();

        // if null it's an IV changed only (key is to be reused).
        if (parameters != null)
        {
            UnderlyingCipher.Init(encrypting, parameters);
        }
        else if (oldEncrypting != encrypting)
        {
            throw new ArgumentException("cannot change encrypting state without providing key.");
        }
    }

    /**
    * return the algorithm name and mode.
    *
    * @return the name of the underlying algorithm followed by "/CBC".
    */
    public string AlgorithmName
    {
        get { return $"{UnderlyingCipher.AlgorithmName}/CBC"; }
    }

    public bool IsPartialBlockOkay
    {
        get { return false; }
    }

    /**
    * return the block size of the underlying cipher.
    *
    * @return the block size of the underlying cipher.
    */
    public int GetBlockSize()
    {
        return UnderlyingCipher.GetBlockSize();
    }

    public int ProcessBlock(byte[] input, int inOff, byte[] output, int outOff)
    {
        return encrypting
            ? EncryptBlock(input.AsSpan(inOff), output.AsSpan(outOff))
            : DecryptBlock(input.AsSpan(inOff), output.AsSpan(outOff));
    }

    public int ProcessBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        return encrypting
            ? EncryptBlock(input, output)
            : DecryptBlock(input, output);
    }

    /**
    * reset the chaining vector back to the IV and reset the underlying
    * cipher.
    */
    public void Reset()
    {
        Array.Copy(IV, 0, cbcV, 0, IV.Length);
        Array.Clear(cbcNextV, 0, cbcNextV.Length);
    }

    private int EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Check.DataLength(input, blockSize, "input buffer too short");
        Check.OutputLength(output, blockSize, "output buffer too short");

        for (var i = 0; i < blockSize; i++)
        {
            cbcV[i] ^= input[i];
        }

        var length = UnderlyingCipher.ProcessBlock(cbcV, output);

        output[..blockSize].CopyTo(cbcV);

        return length;
    }

    private int DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Check.DataLength(input, blockSize, "input buffer too short");
        Check.OutputLength(output, blockSize, "output buffer too short");

        input[..blockSize].CopyTo(cbcNextV);

        var length = UnderlyingCipher.ProcessBlock(input, output);

        for (var i = 0; i < blockSize; i++)
        {
            output[i] ^= cbcV[i];
        }

        (cbcV, cbcNextV) = (cbcNextV, cbcV);

        return length;
    }
}