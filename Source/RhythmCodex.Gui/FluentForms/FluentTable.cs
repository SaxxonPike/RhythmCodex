using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentTable : FluentControl<TableLayoutPanel>
    {
        public FluentTable()
        {
            Dock = DockStyle.Fill;
        }

        public List<ColumnStyle> Columns { get; set; }
        public List<RowStyle> Rows { get; set; }

        protected override Control OnBuild(FluentState state)
        {
            var result = new TableLayoutPanel();
            SetDefault(result);

            if (Columns != null)
            {
                foreach (var style in Columns)
                    result.ColumnStyles.Add(style);
                result.ColumnCount = Columns.Count;
            }

            if (Rows != null)
            {
                foreach (var style in Rows)
                    result.RowStyles.Add(style);
                result.RowCount = Rows.Count;
            }
            
            var children = BuildChildren(state);
            if (children != null)
                result.Controls.AddRange(children.ToArray());

            UpdateMap(state, result);
            return result;
        }
    }

    public class FluentCheckbox : FluentControl<CheckBox, CheckState>
    {
        public FluentCheckbox()
        {
            AutoSize = true;
        }
        
        protected override Control OnBuild(FluentState state)
        {
            var result = new CheckBox();
            SetDefault(result);

            result.CheckState = InitialValue;
            
            UpdateMap(state, result);
            return result;
        }
    }
}