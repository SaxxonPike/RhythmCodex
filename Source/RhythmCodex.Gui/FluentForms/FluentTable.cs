using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

public class FluentTable : FluentControl<TableLayoutPanel>
{
    public FluentTable()
    {
        Dock = DockStyle.Fill;
    }

    public List<ColumnStyle> Columns { get; set; } = [];
    public List<RowStyle> Rows { get; set; } = [];

    protected override Control OnBuild(FluentState state)
    {
        var result = new TableLayoutPanel();
        SetDefault(result);

        if (Columns.Count > 0)
        {
            foreach (var style in Columns)
                result.ColumnStyles.Add(style);
            result.ColumnCount = Columns.Count;
        }

        if (Rows.Count > 0)
        {
            foreach (var style in Rows)
                result.RowStyles.Add(style);
            result.RowCount = Rows.Count;
        }
            
        var children = BuildChildren(state);
        result.Controls.AddRange(children.ToArray());

        UpdateMap(state, result);
        return result;
    }
}