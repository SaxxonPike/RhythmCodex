using System;
using RhythmCodex.Plugin.BouncyCastle.Lib.util;

namespace RhythmCodex.Plugin.BouncyCastle.Lib.crypto.parameters;

public class ParametersWithIV
    : ICipherParameters
{
    private readonly ICipherParameters parameters;
    private readonly byte[] iv;

    public ParametersWithIV(ICipherParameters parameters,
        byte[] iv)
        : this(parameters, iv, 0, iv.Length)
    {
    }

    public ParametersWithIV(ICipherParameters parameters,
        byte[] iv, int ivOff, int ivLen)
    {
        // NOTE: 'parameters' may be null to imply key re-use
        if (iv == null)
            throw new ArgumentNullException("iv");

        this.parameters = parameters;
        this.iv = Arrays.CopyOfRange(iv, ivOff, ivOff + ivLen);
    }

    public byte[] GetIV()
    {
        return (byte[])iv.Clone();
    }

    public ICipherParameters Parameters
    {
        get { return parameters; }
    }
}