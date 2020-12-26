using System;

namespace RhythmCodex.Heuristics
{
    public interface IHeuristicReader
    {
        long? Position { get; set; }
        long? Length { get; }
        ReadOnlySpan<byte> Read(int count);
        int Read(byte[] buffer, int offset, int length);
        byte ReadByte();
        int ReadInt();
        void Skip(int count);
    }
}