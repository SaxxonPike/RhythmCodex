namespace RhythmCodex.Cd.Model;

public class CdSector : ICdSector
{
    public int Number { get; set; }
    public byte[] Data { get; set; }
}