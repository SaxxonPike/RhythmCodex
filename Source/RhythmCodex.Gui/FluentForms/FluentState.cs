using System;
using System.Collections.Generic;

namespace RhythmCodex.Gui.FluentForms;

public class FluentState
{
    public FluentState()
    {
        Map = new Dictionary<string, object>();
        Callbacks = new List<Action>();
    }

    public List<Action> Callbacks { get; }
    public Dictionary<string, object> Map { get; }
}