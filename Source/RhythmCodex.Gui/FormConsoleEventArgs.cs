using System;

namespace RhythmCodex.Gui;

public class FormConsoleEventArgs : EventArgs
{
    public FormConsoleEventArgs(string text) =>
        Text = text;
        
    public string Text { get; }
}