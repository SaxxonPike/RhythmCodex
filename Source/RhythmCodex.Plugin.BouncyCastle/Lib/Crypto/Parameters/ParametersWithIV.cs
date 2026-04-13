using System;

// ReSharper disable once CheckNamespace

namespace Org.BouncyCastle.Crypto.Parameters;

internal class ParametersWithIv(ICipherParameters parameters, ReadOnlySpan<byte> iv)
    : ICipherParameters
{
    private readonly byte[] _iv = iv.ToArray();

    public void CopyIvTo(Span<byte> buf, int off, int len) =>
        _iv.CopyTo(buf.Slice(off, len));

    public int IvLength => _iv.Length;

    public ICipherParameters Parameters { get; } = parameters;
}