using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentEmpty : FluentControl<Control>
    {
        protected override Control OnBuild(FluentState state)
        {
            var result = new Control();
            SetDefault(result);
            UpdateMap(state, result);
            return result;
        }
    }
}