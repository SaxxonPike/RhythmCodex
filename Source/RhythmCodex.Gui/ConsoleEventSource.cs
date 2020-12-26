using System;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Gui
{
    public class ConsoleEventSource : IConsole, IConsoleEventSource
    {
        public event EventHandler<FormConsoleEventArgs> Logged; 
        
        public void Write(string text)
        {
            Logged?.Invoke(this, new FormConsoleEventArgs(text));
        }

        public void WriteLine(params string[] text)
        {
            foreach (var line in text)
                Logged?.Invoke(this, new FormConsoleEventArgs(line + Environment.NewLine));
        }
    }
}