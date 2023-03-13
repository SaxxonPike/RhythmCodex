namespace RhythmCodex.Digital573.Providers;

public interface IDigital573AudioKeyProvider
{
    int[] Get(byte[] source);
}