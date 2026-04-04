using Org.BouncyCastle.Utilities;

// ReSharper disable once CheckNamespace

namespace Org.BouncyCastle.Crypto.Parameters;

public class KeyParameter(byte[] key) : ICipherParameters
{
    private readonly byte[] m_key = Arrays.CopyBuffer(key);

    public byte[] GetKey() => Arrays.InternalCopyBuffer(m_key);
}