using System;

// ReSharper disable once CheckNamespace

namespace Org.BouncyCastle.Utilities;

/// <summary> General array utilities.</summary>
public static class Arrays
{
    public static void Fill(byte[] buf, byte b)
    {
        Fill<byte>(buf, b);
    }

    public static void Fill<T>(T[] ts, T t)
    {
        Array.Fill(ts, t);
    }

    public static byte[] CopyOf(byte[] data, int newLength)
    {
        var tmp = new byte[newLength];
        Array.Copy(data, 0, tmp, 0, System.Math.Min(newLength, data.Length));
        return tmp;
    }

    public static T[] CopyBuffer<T>(T[] buf)
    {
        ValidateBuffer(buf);
        return InternalCopyBuffer(buf);
    }

    public static void CopyBufferToSegment<T>(T[] srcBuf, T[] dstBuf, int dstOff, int dstLen)
    {
        ValidateBuffer(srcBuf);
        ValidateSegment(dstBuf, dstOff, dstLen);
        InternalCopyBufferToSegment(srcBuf, dstBuf, dstOff, dstLen);
    }

    internal static T[] InternalCopyBuffer<T>(T[] buf) => (T[])buf.Clone();

    internal static void InternalCopyBufferToSegment<T>(T[] srcBuf, T[] dstBuf, int dstOff, int dstLen)
    {
        if (srcBuf.Length != dstLen)
            throw new ArgumentOutOfRangeException(nameof(dstLen));

        Array.Copy(srcBuf, 0, dstBuf, dstOff, dstLen);
    }

    public static void ValidateBuffer<T>(T[] buf)
    {
        if (buf == null)
            throw new ArgumentNullException(nameof(buf));
    }

    public static void ValidateSegment<T>(T[] buf, int off, int len)
    {
        if (buf == null)
            throw new ArgumentNullException(nameof(buf));
        var available = buf.Length - off;
        if ((off | available) < 0)
            throw new ArgumentOutOfRangeException(nameof(off));
        var remaining = available - len;
        if ((len | remaining) < 0)
            throw new ArgumentOutOfRangeException(nameof(len));
    }
}