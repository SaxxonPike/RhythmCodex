using System.IO;

namespace RhythmCodex.Infrastructure;

/// <summary>
/// Base class for all stream wrappers. Passes through all required Stream functionality to a base stream.
/// </summary>
public abstract class StreamWrapper(Stream baseStream) : Stream
{
    protected readonly Stream BaseStream = baseStream;

    public override void Flush() => BaseStream.Flush();
    public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);
    public override void SetLength(long value) => BaseStream.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);
    public override bool CanRead => BaseStream.CanRead;
    public override bool CanSeek => BaseStream.CanSeek;
    public override bool CanWrite => BaseStream.CanWrite;
    public override long Length => BaseStream.Length;

    public override long Position
    {
        get => BaseStream.Position;
        set => BaseStream.Position = value;
    }

    protected override void Dispose(bool disposing) => BaseStream.Dispose();
}