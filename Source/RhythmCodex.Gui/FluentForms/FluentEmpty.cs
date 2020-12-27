using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentEmpty : FluentControl<Control>
    {
        public FluentEmpty()
        {
            Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        }
        
        protected override Control OnBuild(FluentState state)
        {
            var result = new Control();
            SetDefault(result);

            result.TabStop = false;

            UpdateMap(state, result);
            return result;
        }
    }
}