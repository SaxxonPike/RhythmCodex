using System;

namespace RhythmCodex.Gui;

public class FormConsoleEventArgs(string text) : EventArgs
{
    public string Text { get; } = text;
}