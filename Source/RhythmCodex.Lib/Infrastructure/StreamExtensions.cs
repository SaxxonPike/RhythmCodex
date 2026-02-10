using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RhythmCodex.Infrastructure;

public static class StreamExtensions
{
    private const int MaxBufferSize = 1 << 20;

    extension(Stream stream)
    {
        public Memory<byte> ReadZeroTerminated()
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

        public long SkipBytes(long length)
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

        public Memory<byte> ReadAllBytes()
        {
            return ReadAllBytes((SpanReadFunc)stream.Read);
        }

        public IEnumerable<string> ReadAllLines()
        {
            var reader = new StreamReader(stream);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    yield break;
                yield return line.Trim();
            }
        }

        public string ReadAllText()
        {
            var reader = new StreamReader(stream, Encoding.UTF8, true);
            return reader.ReadToEnd();
        }

        public byte[] TryRead(int offset, int count)
        {
            var result = new byte[count];
            var read = stream.TryRead(result, offset, count);
            if (read < result.Length)
                Array.Resize(ref result, read);
            return result;
        }

        public int TryRead(Span<byte> buffer, int length) =>
            stream.ReadAtLeast(buffer, length);

        public int TryRead(Span<byte> buffer, int offset, int length) =>
            stream.ReadAtLeast(buffer.Slice(offset, length), length);

        public long Skip(long length)
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
                var bytesRead = stream.TryRead(buffer, 0, (int)Math.Min(length, bufferLength));
                if (bytesRead == 0)
                    break;
                length -= bytesRead;
                result += bytesRead;
            }

            return result;
        }
    }

    extension(MemoryStream mem)
    {
        public Span<byte> AsSpan() =>
            mem.GetBuffer().AsSpan(0, (int)mem.Length);

        public Span<byte> AsSpan(int offset) =>
            mem.AsSpan()[offset..];

        public Span<byte> AsSpan(Range range) =>
            mem.AsSpan()[range];

        public Memory<byte> AsMemory() =>
            mem.GetBuffer().AsMemory(0, (int)mem.Length);

        public Memory<byte> AsMemory(int offset) =>
            mem.AsMemory()[offset..];

        public Memory<byte> AsMemory(Range range) =>
            mem.AsMemory()[range];
    }

    public delegate int SpanReadFunc(Span<byte> buffer);

    public static Memory<byte> ReadAllBytes(SpanReadFunc readFunc)
    {
        byte[]? buffer = null;
        var ms = new MemoryStream();

        try
        {
            buffer = ArrayPool<byte>.Shared.Rent(MaxBufferSize);

            while (true)
            {
                var actualBytesRead = readFunc(buffer);
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
}