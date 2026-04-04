using Org.BouncyCastle.Utilities;

// ReSharper disable once CheckNamespace

namespace Org.BouncyCastle.Crypto.Parameters;

public class ParametersWithIV(ICipherParameters parameters, byte[] iv) : ICipherParameters
{
    private readonly byte[] m_iv = Arrays.CopyBuffer(iv);

    public void CopyIVTo(byte[] buf, int off, int len) => Arrays.CopyBufferToSegment(m_iv, buf, off, len);

    public int IVLength => m_iv.Length;

    public ICipherParameters Parameters { get; } = parameters;
}