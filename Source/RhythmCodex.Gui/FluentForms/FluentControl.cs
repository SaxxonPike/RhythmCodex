using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public abstract class FluentControl : FluentComponent
    {
        public AnchorStyles Anchor { get; set; }
        public DockStyle Dock { get; set; }
        public List<FluentControl> Controls { get; set; }
        public abstract Type Type { get; }
        public Padding Padding { get; set; }
        public Font Font { get; set; }
        public int RowSpan { get; set; } = 1;
        public int ColumnSpan { get; set; } = 1;

        protected abstract Control OnBuild(FluentState state);

        public virtual Control Build(FluentState state)
        {
            var result = OnBuild(state);
            if (RowSpan != 1 || ColumnSpan != 1)
            {
                state.Callbacks.Add(() =>
                {
                    if (result?.Parent is TableLayoutPanel tlp)
                    {
                        tlp.SetRowSpan(result, RowSpan);
                        tlp.SetColumnSpan(result, ColumnSpan);
                    }
                });
            }
            return result;
        }

        protected Control[] BuildChildren(FluentState state) =>
            Controls?.Select(c => c?.Build(state)).Where(c => c != null).ToArray();

        protected override void SetDefault(Control control)
        {
            base.SetDefault(control);
            control.Anchor = Anchor;
            control.Dock = Dock;
            control.Padding = Padding;

            if (Font != null)
                control.Font = Font;
        }
    }

    public abstract class FluentControl<TControl> : FluentControl
        where TControl : Control
    {
        public override Type Type => typeof(TControl);

        public Action<FluentContext<FluentControl<TControl>, TControl>> AfterBuild { get; set; }
        
        public Action OnClick { get; set; }
        public Action OnEnter { get; set; }
        public Action OnLeave { get; set; }

        public override Control Build(FluentState state)
        {
            var result = base.Build(state);
            state.Callbacks.Add(() => AfterBuild?.Invoke(
                new FluentContext<FluentControl<TControl>, TControl>(this, (TControl) result, state)));
            return result;
        }

        protected override void SetDefault(Control control)
        {
            base.SetDefault(control);

            if (OnClick != null)
                control.Click += (o, e) => OnClick?.Invoke();
            if (OnEnter != null)
                control.Enter += (o, e) => OnEnter?.Invoke();
            if (OnLeave != null)
                control.Leave += (o, e) => OnLeave?.Invoke();
        }

        protected override void SetDefault(ToolStripItem toolStripItem)
        {
            base.SetDefault(toolStripItem);
            if (OnClick != null)
                toolStripItem.Click += (o, e) => OnClick?.Invoke();
        }
    }

    public abstract class FluentControl<TControl, TValue> : FluentControl<TControl>
        where TControl : Control
    {
        public Action<TControl, TValue> OnChange { get; set; }
    }
}