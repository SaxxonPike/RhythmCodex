using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

public class FluentCheckbox : FluentControl<CheckBox, CheckState>
{
    public FluentCheckbox()
    {
        AutoSize = true;
    }
        
    protected override Control OnBuild(FluentState state)
    {
        var result = new CheckBox();
        SetDefault(result);

        result.CheckState = InitialValue;
            
        UpdateMap(state, result);
        return result;
    }
}