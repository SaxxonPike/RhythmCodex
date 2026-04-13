using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace

namespace Org.BouncyCastle.Crypto.Modes;

/**
* A Cipher Text Stealing (CTS) mode cipher. CTS allows block ciphers to
* be used to produce cipher text which is the same outLength as the plain text.
*/
internal sealed class CtsBlockCipher
    : BufferedBlockCipher
{
    private readonly int _blockSize;

    /**
    * Create a buffered block cipher that uses Cipher Text Stealing
    *
    * @param cipher the underlying block cipher this buffering object wraps.
    */
    public CtsBlockCipher(IBlockCipherMode cipherMode)
    {
        if (cipherMode is not CbcBlockCipher)
            throw new ArgumentException("CtsBlockCipher can only accept ECB, or CBC ciphers");

        CipherMode = cipherMode;

        _blockSize = cipherMode.GetBlockSize();

        Buf = new byte[_blockSize * 2];
        BufOff = 0;
    }

    /**
    * return the size of the output buffer required for an update of 'length' bytes.
    *
    * @param length the outLength of the input.
    * @return the space required to accommodate a call to update
    * with length bytes of input.
    */
    public override int GetUpdateOutputSize(int length) =>
        GetFullBlocksSize(totalSize: BufOff + length - 1, blockSize: Buf.Length);

    protected override int ProcessBytes(ReadOnlySpan<byte> input, Span<byte> output)
    {
        var resultLen = 0;
        var available = Buf.Length - BufOff;

        if (input.Length > available)
        {
            var updateOutputSize = GetUpdateOutputSize(input.Length);
            Debug.Assert(updateOutputSize >= _blockSize);
            Check.OutputLength(output, updateOutputSize, "output buffer too short");

            input[..available].CopyTo(Buf.AsSpan(BufOff));
            input = input[available..];

            // Handle destructive overlap by copying the remaining input
            if (output[.._blockSize].Overlaps(input))
            {
                input = input.ToArray();
            }

            resultLen = CipherMode.ProcessBlock(Buf, output);
            Array.Copy(Buf, _blockSize, Buf, 0, _blockSize);
            BufOff = _blockSize;

            while (input.Length > _blockSize)
            {
                resultLen += CipherMode.ProcessBlock(Buf, output[resultLen..]);
                input[.._blockSize].CopyTo(Buf);
                input = input[_blockSize..];
            }
        }

        input.CopyTo(Buf.AsSpan(BufOff));
        BufOff += input.Length;
        return resultLen;
    }

    protected override int DoFinal(Span<byte> output)
    {
        try
        {
            Check.DataLength(BufOff < _blockSize, "need at least one block of input for CTS");
            Check.OutputLength(output, BufOff, "output buffer too short");

            if (ForEncryption)
            {
                CipherMode.ProcessBlock(Buf, Buf);

                for (var i = BufOff; i < Buf.Length; ++i)
                {
                    Buf[i] = Buf[i - _blockSize];
                }
                for (var i = _blockSize; i < BufOff; ++i)
                {
                    Buf[i] ^= Buf[i - _blockSize];
                }

                CipherMode.UnderlyingCipher.ProcessBlock(Buf.AsSpan(_blockSize), output);
                Buf.AsSpan(0, BufOff - _blockSize).CopyTo(output[_blockSize..]);
            }
            else
            {
                CipherMode.UnderlyingCipher.ProcessBlock(Buf, Buf);

                for (var i = _blockSize; i < BufOff; ++i)
                {
                    var t = Buf[i - _blockSize];
                    Buf[i - _blockSize] = Buf[i];
                    Buf[i] ^= t;
                }

                CipherMode.ProcessBlock(Buf, output);
                Buf.AsSpan(_blockSize, BufOff - _blockSize).CopyTo(output[_blockSize..]);
            }

            return BufOff;
        }
        finally
        {
            Reset();
        }
    }
}