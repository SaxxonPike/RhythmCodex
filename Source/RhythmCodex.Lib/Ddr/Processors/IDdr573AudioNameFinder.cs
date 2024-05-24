namespace RhythmCodex.Ddr.Processors;

public interface IDdr573AudioNameFinder
{
    string? GetName(string sourceName);
    string GetPath(string sourceName);
}