using System;
using System.Runtime.InteropServices;

namespace RhythmCodex.Plugin.CSCore.Lib.Utils;

/// <summary>
/// This class is based on the CUETools.NET BitReader (see http://sourceforge.net/p/cuetoolsnet/code/ci/default/tree/CUETools.Codecs/BitReader.cs)
/// The author "Grigory Chudov" explicitly gave the permission to use the source as part of the cscore source code which got licensed under the ms-pl.
/// </summary>
internal unsafe class BitReader : IDisposable
{
    private int _bitoffset;
    private byte* _buffer;
    private GCHandle _hBuffer;

    public BitReader(byte[] buffer, int offset)
    {
        if (buffer == null || buffer.Length <= 0)
            throw new ArgumentException("buffer is null or has no elements", nameof(buffer));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        _hBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        _buffer = Buffer = (byte*) _hBuffer.AddrOfPinnedObject().ToPointer() + offset;

        Cache = PeekCache();
    }

    public BitReader(byte* buffer, int offset)
    {
        if (new IntPtr(buffer) == IntPtr.Zero)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));

        var byteoffset = offset / 8;

        _buffer = Buffer = buffer + byteoffset;
        _bitoffset = offset % 8;

        Cache = PeekCache();
    }

    protected internal uint Cache { get; private set; }

    protected internal int CacheSigned => (int) Cache;

    public byte* Buffer { get; }

    public IntPtr BufferPtr => new(Buffer);

    public int Position { get; private set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private uint PeekCache()
    {
        unchecked
        {
            var ptr = _buffer;
            uint result = *(ptr++);
            result = (result << 8) + *(ptr++);
            result = (result << 8) + *(ptr++);
            result = (result << 8) + *(ptr++);

            return result << _bitoffset;
        }
    }

    public void SeekBytes(int bytes)
    {
        if (bytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        SeekBits(bytes * 8);
    }

    public void SeekBits(int bits)
    {
        if (bits <= 0)
            throw new ArgumentOutOfRangeException(nameof(bits));

        var tmp = _bitoffset + bits;
        _buffer += tmp >> 3; //skip bytes
        _bitoffset = tmp & 7; //bitoverflow -> max 7 bit

        Cache = PeekCache();

        Position += tmp >> 3;
    }

    public uint ReadBits(int bits)
    {
        if (bits <= 0 || bits > 32)
            throw new ArgumentOutOfRangeException(nameof(bits), "bits has to be a value between 1 and 32");

        var result = Cache >> 32 - bits;
        if (bits <= 24)
        {
            SeekBits(bits);
            return result;
        }

        SeekBits(24);
        result |= Cache >> 56 - bits;
        SeekBits(bits - 24);

        return result;
    }

    public int ReadBitsSigned(int bits)
    {
        if (bits <= 0 || bits > 32)
            throw new ArgumentOutOfRangeException(nameof(bits), "bits has to be a value between 1 and 32");

        var result = (int) ReadBits(bits);
        result <<= (32 - bits);
        result >>= (32 - bits);
        return result;
    }

    public ulong ReadBits64(int bits)
    {
        if (bits <= 0 || bits > 64)
            throw new ArgumentOutOfRangeException(nameof(bits), "bits has to be a value between 1 and 64");

        ulong result = ReadBits(Math.Min(24, bits));
        if (bits <= 24)
            return result;

        bits -= 24;
        result = (result << bits) | ReadBits(Math.Min(24, bits));
        if (bits <= 24)
            return result;

        bits -= 24;
        return (result << bits) | ReadBits(bits);
    }

    public long ReadBits64Signed(int bits)
    {
        if (bits <= 0 || bits > 64)
            throw new ArgumentOutOfRangeException(nameof(bits), "bits has to be a value between 1 and 64");

        var result = (long) ReadBits64(bits);
        result <<= (64 - bits);
        result >>= (64 - bits);
        return result;
    }

    public short ReadInt16()
    {
        return (short) ReadBitsSigned(16);
    }

    public ushort ReadUInt16()
    {
        return (ushort) ReadBits(16);
    }

    public int ReadInt32()
    {
        return ReadBitsSigned(32);
    }


    public uint ReadUInt32()
    {
        return ReadBits(32);
    }

    public ulong ReadUInt64()
    {
        return ReadBits64(64);
    }

    public long ReadInt64()
    {
        return ReadBits64Signed(64);
    }

    public bool ReadBit()
    {
        return ReadBitI() == 1;
    }

    public int ReadBitI()
    {
        var result = Cache >> 31;
        SeekBits(1);
        return (int) result;
    }

    public void Flush()
    {
        if (_bitoffset > 0 && _bitoffset <= 8)
            SeekBits(8 - _bitoffset);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_hBuffer.IsAllocated)
            _hBuffer.Free();
    }

    ~BitReader()
    {
        Dispose(false);
    }
}