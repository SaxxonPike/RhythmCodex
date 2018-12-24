using System;

namespace RhythmCodex.Infrastructure
{
    public interface IHeuristic
    {
        string Description { get; }
        string FileExtension { get; }
        bool IsMatch(ReadOnlySpan<byte> data);
    }
}