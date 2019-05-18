namespace RhythmCodex.Ddr.Providers
{
    public interface IDdr573AudioKeyProvider
    {
        int[] Get(byte[] source);
    }
}