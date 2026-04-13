using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace

namespace Org.BouncyCastle.Crypto;

internal abstract class BufferedCipherBase
{
    protected abstract int ProcessBytes(ReadOnlySpan<byte> input, Span<byte> output);

    protected abstract int DoFinal(Span<byte> output);

    public int DoFinal(ReadOnlySpan<byte> input, Span<byte> output)
    {
        var len = ProcessBytes(input, output);
        len += DoFinal(output[len..]);
        return len;
    }

    internal static int GetFullBlocksSize(int totalSize, int blockSize)
    {
        Debug.Assert(blockSize > 0);

        if (totalSize < 0)
            return 0;

        var blockSizeMask = blockSize - 1;
        if ((blockSize & blockSizeMask) == 0)
            return totalSize & ~blockSizeMask;

        return totalSize - totalSize % blockSize;
    }
}