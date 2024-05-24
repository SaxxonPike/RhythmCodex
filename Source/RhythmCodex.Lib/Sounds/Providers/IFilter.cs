namespace RhythmCodex.Sounds.Providers;

public interface IFilter
{
    string Name { get; }
    int Priority { get; }
    FilterType Type { get; }
    IFilterContext Create(double sampleRate, double cutoff);
}