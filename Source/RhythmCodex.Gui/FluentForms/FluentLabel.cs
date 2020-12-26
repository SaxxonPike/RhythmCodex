using System.Drawing;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentLabel : FluentControl<Label>
    {
        public FluentLabel()
        {
            AutoSize = true;
            Align = ContentAlignment.TopLeft;
        }

        public ContentAlignment Align { get; set; }

        protected override Control OnBuild(FluentState state)
        {
            var result = new Label();
            SetDefault(result);

            result.TextAlign = Align;

            UpdateMap(state, result);
            return result;
        }
    }
}