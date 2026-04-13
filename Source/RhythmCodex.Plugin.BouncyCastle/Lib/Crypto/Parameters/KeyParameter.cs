using System;
using System.Linq;

// ReSharper disable once CheckNamespace

namespace Org.BouncyCastle.Crypto.Parameters;

internal class KeyParameter(ReadOnlySpan<byte> key) : ICipherParameters
{
    private readonly byte[] _key = key.ToArray();

    public byte[] GetKey() => _key.ToArray();
}