using System.Linq;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentPanel : FluentControl<Panel>
    {
        public FluentPanel()
        {
            Dock = DockStyle.Fill;
            Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        }
        
        protected override Control OnBuild(FluentState state)
        {
            var result = new Panel();
            SetDefault(result);
            var children = BuildChildren(state);
            if (children != null)
                result.Controls.AddRange(children.ToArray());
            return result;
        }
    }
}