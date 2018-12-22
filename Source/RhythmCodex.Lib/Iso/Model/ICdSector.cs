namespace RhythmCodex.Iso.Model
{
    public interface ICdSector
    {
        int Number { get; }
        byte[] Data { get; }
    }
}