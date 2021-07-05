using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentTabPage : FluentControl<TabPage>
    {
        public bool AutoScroll { get; set; } = true;
        
        protected override Control OnBuild(FluentState state)
        {
            var result = new TabPage();
            SetDefault(result);

            result.AutoScroll = AutoScroll;
            result.HorizontalScroll.Enabled = false;
            result.VerticalScroll.Enabled = AutoScroll;
            
            var children = BuildChildren(state);
            if (children != null)
                result.Controls.AddRange(children);

            UpdateMap(state, result);
            return result;
        }
    }
}