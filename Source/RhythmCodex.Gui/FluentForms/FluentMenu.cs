using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

public class FluentMenu : FluentControl<MenuStrip>
{
    public FluentMenu()
    {
        Dock = DockStyle.Top;
        AutoSize = true;
    }

    public List<FluentMenuItem> Items { get; set; } = [];

    protected override Control OnBuild(FluentState state)
    {
        var result = new MenuStrip();
        SetDefault(result);
        result.Items.AddRange(Items.Select(i => i.Build(state)).ToArray());
        UpdateMap(state, result);
        return result;
    }
}