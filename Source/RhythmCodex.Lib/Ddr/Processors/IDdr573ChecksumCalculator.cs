namespace RhythmCodex.Ddr.Processors
{
    public interface IDdr573ChecksumCalculator
    {
        int CalculateChecksum(byte[] data);
    }
}