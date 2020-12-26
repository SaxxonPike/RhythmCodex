using System.Drawing;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentTextBox : FluentControl<TextBox, string>
    {
        protected override Control OnBuild(FluentState state)
        {
            var result = new TextBox();
            SetDefault(result);
            result.Multiline = MultiLine;
            result.ScrollBars = ScrollBars;
            result.WordWrap = WordWrap;
            if (OnChange != null)
                result.TextChanged += (o, e) => OnChange?.Invoke(result, result.Text);
            UpdateMap(state, result);
            return result;
        }
        
        public bool MultiLine { get; set; }
        public ScrollBars ScrollBars { get; set; }
        public bool WordWrap { get; set; }
    }
}