using System;
using System.Diagnostics;
using Org.BouncyCastle.Crypto.Modes;

// ReSharper disable CheckNamespace

namespace Org.BouncyCastle.Crypto;

/**
* A wrapper class that allows block ciphers to be used to process data in
* a piecemeal fashion. The BufferedBlockCipher outputs a block only when the
* buffer is full and more data is being added, or on a doFinal.
* <p>
* Note: in the case where the underlying cipher is either a CFB cipher or an
* OFB one the last block may not be a multiple of the block size.
* </p>
*/
internal abstract class BufferedBlockCipher
    : BufferedCipherBase
{
    internal byte[] Buf = [];
    internal int BufOff;
    internal bool ForEncryption;
    internal IBlockCipherMode CipherMode;

    /**
    * constructor for subclasses
    */
    protected BufferedBlockCipher()
    {
    }

    /**
    * initialise the cipher.
    *
    * @param forEncryption if true the cipher is initialised for
    *  encryption, if false for decryption.
    * @param param the key and other data required by the cipher.
    * @exception ArgumentException if the parameters argument is
    * inappropriate.
    */
    // Note: This doubles as the Init in the event that this cipher is being used as an IWrapper
    public void Init(bool forEncryption, ICipherParameters parameters)
    {
        ForEncryption = forEncryption;

        // TODO[api] Redundantly resets the cipher mode
        Reset();

        CipherMode.Init(forEncryption, parameters);
    }

    /**
    * return the size of the output buffer required for an update
    * an input of len bytes.
    *
    * @param len the length of the input.
    * @return the space required to accommodate a call to update
    * with len bytes of input.
    */
    public virtual int GetUpdateOutputSize(int length) =>
        GetFullBlocksSize(totalSize: BufOff + length, blockSize: Buf.Length);

    protected override int ProcessBytes(ReadOnlySpan<byte> input, Span<byte> output)
    {
        var resultLen = 0;
        var blockSize = Buf.Length;
        var available = blockSize - BufOff;

        if (input.Length >= available)
        {
            var updateOutputSize = GetUpdateOutputSize(input.Length);
            Debug.Assert(updateOutputSize >= blockSize);
            Check.OutputLength(output, updateOutputSize, "output buffer too short");

            input[..available].CopyTo(Buf.AsSpan(BufOff));
            input = input[available..];

            // Handle destructive overlap by copying the remaining input
            if (output[..blockSize].Overlaps(input))
            {
                input = input.ToArray();
            }

            resultLen = CipherMode.ProcessBlock(Buf, output);
            BufOff = 0;

            while (input.Length >= blockSize)
            {
                resultLen += CipherMode.ProcessBlock(input, output[resultLen..]);
                input = input[blockSize..];
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
            if (BufOff != 0)
            {
                Check.DataLength(!CipherMode.IsPartialBlockOkay, "data not block size aligned");
                Check.OutputLength(output, BufOff, "output buffer too short for DoFinal()");

                // NB: Can't copy directly, or we may write too much output
                CipherMode.ProcessBlock(Buf, Buf);
                Buf.AsSpan(0, BufOff).CopyTo(output);
            }

            return BufOff;
        }
        finally
        {
            Reset();
        }
    }

    /**
    * Reset the buffer and cipher. After resetting the object is in the same
    * state as it was after the last init (if there was one).
    */
    protected void Reset()
    {
        Array.Clear(Buf, 0, Buf.Length);
        BufOff = 0;

        CipherMode.Reset();
    }
}