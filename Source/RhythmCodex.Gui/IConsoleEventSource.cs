using System;

namespace RhythmCodex.Gui
{
    public interface IConsoleEventSource
    {
        event EventHandler<FormConsoleEventArgs> Logged;
    }
}