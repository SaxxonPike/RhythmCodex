namespace RhythmCodex.Digital573.Models;

public class Digital573Audio
{
    public required byte[] Data { get; init; }
    public required byte[] Key { get; init; }
    public required int Counter { get; init; }
}