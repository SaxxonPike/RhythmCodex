using System.Linq;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

public class FluentPanel : FluentControl<Panel>
{
    public bool AutoScroll { get; set; }
    public AutoSizeMode AutoSizeMode { get; set; }
        
    protected override Control OnBuild(FluentState state)
    {
        var result = new Panel();
        SetDefault(result);

        result.AutoScroll = AutoScroll;
        result.AutoSizeMode = AutoSizeMode;

        var children = BuildChildren(state);
        if (children != null)
            result.Controls.AddRange(children.ToArray());
        return result;
    }
}