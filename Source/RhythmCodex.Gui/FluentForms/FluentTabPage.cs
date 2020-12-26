using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentTabPage : FluentControl<TabPage>
    {
        protected override Control OnBuild(FluentState state)
        {
            var result = new TabPage();
            SetDefault(result);

            var children = BuildChildren(state);
            if (children != null)
                result.Controls.AddRange(children);

            UpdateMap(state, result);
            return result;
        }
    }
}