namespace RhythmCodex.Cd.Model
{
    public interface ICdSector
    {
        int Number { get; }
        byte[] Data { get; }
    }
}