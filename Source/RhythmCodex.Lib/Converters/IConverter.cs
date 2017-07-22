namespace RhythmCodex.Converters
{
    public interface IConverter<in TFrom, out TTo>
    {
        TTo Convert(TFrom data);
    }
}
