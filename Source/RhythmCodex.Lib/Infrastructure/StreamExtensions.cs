using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RhythmCodex.Infrastructure;

public static class StreamExtensions
{
    private const int MaxBufferSize = 1 << 20;

    public static Memory<byte> ReadZeroTerminated(this Stream stream)
    {
        var ms = new MemoryStream();
        while (true)
        {
            var b = stream.ReadByte();
            if (b <= 0)
                return ms.GetBuffer().AsMemory(0, (int)ms.Length);
            ms.WriteByte(unchecked((byte)b));
        }
    }

    public static long SkipBytes(this Stream stream, long length)
    {
        using var bufferHandle = length < MaxBufferSize
            ? MemoryPool<byte>.Shared.Rent((int)length)
            : MemoryPool<byte>.Shared.Rent();
        var buffer = bufferHandle.Memory.Span;
        var remaining = length;
        var offset = 0;

        while (remaining > 0)
        {
            var size = (int)Math.Min(length, buffer.Length);
            var amount = stream.ReadAtLeast(buffer, size, false);
            if (amount < 1)
                break;
            remaining -= amount;
            offset += amount;
        }

        return offset;
    }

    public static Memory<byte> ReadAllBytes(Func<byte[], int, int, int> readFunc)
    {
        byte[]? buffer = null;
        var ms = new MemoryStream();

        try
        {
            buffer = ArrayPool<byte>.Shared.Rent(MaxBufferSize);

            while (true)
            {
                var actualBytesRead = readFunc(buffer, 0, buffer.Length);
                if (actualBytesRead < 1)
                    break;

                ms.Write(buffer.AsSpan(0, actualBytesRead));
            }

            return ms.GetBuffer().AsMemory(0, (int)ms.Length);
        }
        finally
        {
            if (buffer != null)
                ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static Memory<byte> ReadAllBytes(this Stream stream)
    {
        return ReadAllBytes(stream.Read);
    }

    public static IEnumerable<string> ReadAllLines(this Stream source)
    {
        var reader = new StreamReader(source);
        while (true)
        {
            var line = reader.ReadLine();
            if (line == null)
                yield break;
            yield return line.Trim();
        }
    }

    public static string ReadAllText(this Stream source)
    {
        var reader = new StreamReader(source, Encoding.UTF8, true);
        return reader.ReadToEnd();
    }

    public static byte[] TryRead(this Stream stream, int offset, int count)
    {
        var result = new byte[count];
        var read = TryRead(stream, result, offset, count);
        if (read < result.Length)
            Array.Resize(ref result, read);
        return result;
    }

    public static int TryRead(this Stream stream, Span<byte> buffer, int length) =>
        stream.ReadAtLeast(buffer, length);

    public static int TryRead(this Stream stream, Span<byte> buffer, int offset, int length) =>
        stream.ReadAtLeast(buffer.Slice(offset, length), length);

    public static long Skip(this Stream stream, long length)
    {
        // If the stream is seekable, using its own seek means the most appropriate method
        // will be used.
        if (stream.CanSeek)
            return stream.Seek(length, SeekOrigin.Current);

        using var bufferOwner = MemoryPool<byte>.Shared.Rent((int)Math.Min(length, 65536));
        var buffer = bufferOwner.Memory.Span;
        var bufferLength = buffer.Length;
        var result = 0;

        while (length > 0)
        {
            var bytesRead = TryRead(stream, buffer, 0, (int)Math.Min(length, bufferLength));
            if (bytesRead == 0)
                break;
            length -= bytesRead;
            result += bytesRead;
        }

        return result;
    }

    public static Span<byte> AsSpan(this MemoryStream mem) =>
        mem.GetBuffer().AsSpan(0, (int)mem.Length);

    public static Span<byte> AsSpan(this MemoryStream mem, int offset) =>
        AsSpan(mem)[offset..];

    public static Span<byte> AsSpan(this MemoryStream mem, Range range) =>
        AsSpan(mem)[range];

    public static Memory<byte> AsMemory(this MemoryStream mem) =>
        mem.GetBuffer().AsMemory(0, (int)mem.Length);

    public static Memory<byte> AsMemory(this MemoryStream mem, int offset) =>
        AsMemory(mem)[offset..];

    public static Memory<byte> AsMemory(this MemoryStream mem, Range range) =>
        AsMemory(mem)[range];
}