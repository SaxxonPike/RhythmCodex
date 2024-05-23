﻿using System;
using System.Collections.Generic;

namespace RhythmCodex.Gui.FluentForms;

public class FluentState
{
    public List<Action> Callbacks { get; } = new();
    public Dictionary<string, object> Map { get; } = new();
}