namespace RhythmCodex.Compressions.Models;

public class HuffmanNode
{
    public uint Bits;
    public uint Count;
    public byte NumBits;
    public HuffmanNode? Parent;
    public uint Weight;
}