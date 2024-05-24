using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

public class FluentProgressBar : FluentControl<ProgressBar>
{
    protected override Control OnBuild(FluentState state)
    {
        var result = new ProgressBar();
        SetDefault(result);
            
        UpdateMap(state, result);
        return result;
    }
}