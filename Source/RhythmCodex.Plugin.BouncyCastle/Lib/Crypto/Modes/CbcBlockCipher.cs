using System;
using Org.BouncyCastle.Crypto.Parameters;

// ReSharper disable CheckNamespace

namespace Org.BouncyCastle.Crypto.Modes;

/**
* implements Cipher-Block-Chaining (CBC) mode on top of a simple cipher.
*/
internal sealed class CbcBlockCipher
    : IBlockCipherMode
{
    private readonly byte[] _iv;
    private byte[] _cbcV, _cbcNextV;
    private readonly int _blockSize;
    private bool _encrypting;

    /**
    * Basic constructor.
    *
    * @param cipher the block cipher to be used as the basis of chaining.
    */
    public CbcBlockCipher(
        IBlockCipher cipher)
    {
        UnderlyingCipher = cipher;
        _blockSize = cipher.GetBlockSize();

        _iv = new byte[_blockSize];
        _cbcV = new byte[_blockSize];
        _cbcNextV = new byte[_blockSize];
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
    * @exception ArgumentException if the 'parameters' argument is
    * inappropriate.
    */
    public void Init(bool forEncryption, ICipherParameters? parameters)
    {
        var oldEncrypting = _encrypting;

        _encrypting = forEncryption;

        if (parameters is ParametersWithIv ivParam)
        {
            if (ivParam.IvLength != _blockSize)
                throw new ArgumentException("initialisation vector must be the same length as block size");

            ivParam.CopyIvTo(_iv, 0, _blockSize);

            parameters = ivParam.Parameters;
        }
        else
        {
            _iv.AsSpan().Clear();
        }

        Reset();

        // if null it's an IV changed only (key is to be reused).
        if (parameters != null)
        {
            UnderlyingCipher.Init(_encrypting, parameters);
        }
        else if (oldEncrypting != _encrypting)
        {
            throw new ArgumentException("cannot change encrypting state without providing key.");
        }
    }

    /**
    * return the algorithm name and mode.
    *
    * @return the name of the underlying algorithm followed by "/CBC".
    */
    public string AlgorithmName => 
        $"{UnderlyingCipher.AlgorithmName}/CBC";

    public bool IsPartialBlockOkay => false;

    /**
    * return the block size of the underlying cipher.
    *
    * @return the block size of the underlying cipher.
    */
    public int GetBlockSize() => 
        UnderlyingCipher.GetBlockSize();

    public int ProcessBlock(ReadOnlySpan<byte> input, Span<byte> output) =>
        _encrypting
            ? EncryptBlock(input, output)
            : DecryptBlock(input, output);

    /**
    * reset the chaining vector back to the IV and reset the underlying
    * cipher.
    */
    public void Reset()
    {
        Array.Copy(_iv, 0, _cbcV, 0, _iv.Length);
        Array.Clear(_cbcNextV, 0, _cbcNextV.Length);
    }

    private int EncryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Check.DataLength(input, _blockSize, "input buffer too short");
        Check.OutputLength(output, _blockSize, "output buffer too short");

        for (var i = 0; i < _blockSize; i++)
        {
            _cbcV[i] ^= input[i];
        }

        var length = UnderlyingCipher.ProcessBlock(_cbcV, output);

        output[.._blockSize].CopyTo(_cbcV);

        return length;
    }

    private int DecryptBlock(ReadOnlySpan<byte> input, Span<byte> output)
    {
        Check.DataLength(input, _blockSize, "input buffer too short");
        Check.OutputLength(output, _blockSize, "output buffer too short");

        input[.._blockSize].CopyTo(_cbcNextV);

        var length = UnderlyingCipher.ProcessBlock(input, output);

        for (var i = 0; i < _blockSize; i++)
        {
            output[i] ^= _cbcV[i];
        }

        (_cbcV, _cbcNextV) = (_cbcNextV, _cbcV);

        return length;
    }
}