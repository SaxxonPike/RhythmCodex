using System;
using System.Collections.Generic;
using RhythmCodex.Gui.FluentForms;

namespace RhythmCodex.Gui.Forms;

public interface IControlFactory
{
    IEnumerable<FluentControl> CreateCheckbox(string? id, string text);
    IEnumerable<FluentControl> CreateDualCheckbox(string id0, string text0, string id1, string text1);
    FluentTable CreateStandardTable(string? id, IEnumerable<FluentControl> controls, bool fill);
    IEnumerable<FluentControl> CreateBigButton(string? id, string text, Action<FluentContext> press);
    IEnumerable<FluentControl> CreateFileSelect(string? id, string text, bool multi);
    IEnumerable<FluentControl> CreateFolderSelect(string? id, string text);
    IEnumerable<FluentControl> CreateSpacer(string? text);
    IEnumerable<FluentControl> CreateTextEntry(string? id, string text);
    IEnumerable<FluentControl> CreateProgress(string? id, string text);
}