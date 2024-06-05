using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

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
            result.TextChanged += (_, _) => OnChange?.Invoke(result, result.Text);

        result.AllowDrop = AllowDrop;

        result.DragEnter += (_, e) =>
        {
            if (result.AllowDrop && (e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false))
                e.Effect = DragDropEffects.Link;
        };

        result.DragDrop += (_, e) =>
        {
            if (result.AllowDrop && (e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false))
                result.Text = string.Join('|', (string[]) e.Data.GetData(DataFormats.FileDrop)!);
        };
            
        UpdateMap(state, result);
        return result;
    }
        
    public bool MultiLine { get; set; }
    public ScrollBars ScrollBars { get; set; }
    public bool WordWrap { get; set; }
    public bool AllowDrop { get; set; }
}