using System;
using System.Buffers;

namespace RhythmCodex.Infrastructure;

public sealed class MemoryOwnerStream(IMemoryOwner<byte> owner, int length) : StreamWrapper(
    new ReadOnlyMemoryStream(owner.Memory[..length]))
{
    public Memory<byte> Memory => owner.Memory[..length];

    public Span<byte> Span => owner.Memory.Span[..length];

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        owner.Dispose();
    }
}