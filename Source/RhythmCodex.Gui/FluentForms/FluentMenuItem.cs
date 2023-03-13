using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

public class FluentMenuItem : FluentComponent
{
    public List<FluentMenuItem> Items { get; set; }

    public Action OnClick { get; set; }
    public Action<FluentContext<FluentMenuItem, ToolStripMenuItem>> AfterBuild { get; set; }

    public ToolStripItem Build(FluentState state)
    {
        var result = new ToolStripMenuItem();
        SetDefault(result);

        if (OnClick != null)
            result.Click += (_, _) => OnClick?.Invoke();
        if (Items != null)
            result.DropDownItems.AddRange(Items.Select(i => i.Build(state)).ToArray());

        UpdateMap(state, result);
        AfterBuild?.Invoke(new FluentContext<FluentMenuItem, ToolStripMenuItem>(this, result, state));

        return result;
    }
}