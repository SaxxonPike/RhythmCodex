using RhythmCodex.Infrastructure;

namespace RhythmCodex.Cli.Helpers
{
    public class Console : IConsole
    {
        public void WriteLine(params string[] text)
        {
            foreach (var line in text)
                System.Console.WriteLine(line);
        }
            

        public void Write(string text) => 
            System.Console.Write(text);
    }
}