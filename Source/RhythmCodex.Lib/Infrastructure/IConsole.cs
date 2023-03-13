namespace RhythmCodex.Infrastructure;

public interface IConsole
{
    void Write(string text);
    void WriteLine(params string[] text);
}