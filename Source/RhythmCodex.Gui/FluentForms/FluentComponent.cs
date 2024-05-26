using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

public abstract class FluentComponent
{
    public string? Id { get; set; }
    public object? Tag { get; set; }
    public string? Text { get; set; }
    public bool AutoSize { get; set; }

    protected void UpdateMap(FluentState state, object result)
    {
        if (Id != null)
            state.Map?.Add(Id, result);
    }
        
    protected virtual void SetDefault(Control control)
    {
        if (Tag != null)
            control.Tag = Tag;

        if (Text != null)
            control.Text = Text;

        if (Id != null)
            control.Name = Id;

        control.AutoSize = AutoSize;
    }

    protected virtual void SetDefault(ToolStripItem toolStripItem)
    {
        if (Tag != null)
            toolStripItem.Tag = Tag;

        if (Text != null)
            toolStripItem.Text = Text;
    }
}