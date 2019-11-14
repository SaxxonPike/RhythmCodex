namespace RhythmCodex.Cli.Helpers
{
    public interface IArgResolver
    {
        string[] GetInputFiles(Args args);
        string GetOutputDirectory(Args args);
    }
}