namespace RhythmCodex.Ddr.Converters
{
    public interface IDdr573ChecksumCalculator
    {
        int CalculateChecksum(byte[] data);
    }
}