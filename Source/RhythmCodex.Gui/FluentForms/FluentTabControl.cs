using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentTabControl : FluentControl<TabControl, int>
    {
        public FluentTabControl()
        {
            Dock = DockStyle.Fill;
        }
        
        public List<FluentTabPage> Items { get; set; }

        protected override Control OnBuild(FluentState state)
        {
            var result = new TabControl();
            SetDefault(result);

            result.TabIndexChanged += (s, e) => OnChange?.Invoke(result, result.TabIndex);
            if (Items != null)
                result.TabPages.AddRange(Items.Select(tab => (TabPage) tab.Build(state)).ToArray());
            
            UpdateMap(state, result);
            return result;
        }
    }
}