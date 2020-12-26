using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentForm : FluentControl<Form>
    {
        protected override Control OnBuild(FluentState state)
        {
            var form = new Form();
            SetDefault(form);

            var children = BuildChildren(state);
            if (children != null)
                form.Controls.AddRange(children);

            UpdateMap(state, form);
            return form;
        }
    }
}