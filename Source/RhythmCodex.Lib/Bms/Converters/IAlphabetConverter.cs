namespace RhythmCodex.Bms.Converters
{
    public interface IAlphabetConverter
    {
        string EncodeHex(int value, int length);
        string EncodeNumeric(int value, int length);
        string EncodeAlphanumeric(int value, int length);
    }
}