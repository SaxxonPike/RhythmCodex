using JetBrains.Annotations;

namespace RhythmCodex.Infrastructure;

[UsedImplicitly]
public class TestConsole : IConsole
{
    public void Write(string text)
    {
        TestContext.Out.Write(text);
    }

    public void WriteLine(params string[] text)
    {
        foreach (var line in text)
            TestContext.Out.WriteLine(line);
    }
}