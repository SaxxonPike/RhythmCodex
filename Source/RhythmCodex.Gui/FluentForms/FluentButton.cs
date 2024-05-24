using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

public class FluentButton : FluentControl<Button>
{
    protected override Control OnBuild(FluentState state)
    {
        var result = new Button();
        SetDefault(result);
        result.Click += (_, _) => OnClick?.Invoke();
        UpdateMap(state, result);
        return result;
    }
}