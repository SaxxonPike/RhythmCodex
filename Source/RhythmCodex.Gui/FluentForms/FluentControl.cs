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
        public Size MinimumSize { get; set; } = Size.Empty;
        public Size MaximumSize { get; set; } = Size.Empty;
        public Color? ForeColor { get; set; }
        public Color? BackColor { get; set; }

        protected abstract Control OnBuild(FluentState state);

        public virtual Control Build(FluentState state, Control parent = null)
        {
            var result = OnBuild(state);
            if (RowSpan != 1 || ColumnSpan != 1)
            {
                state.Callbacks.Add(() =>
                {
                    var p = result?.Parent ?? parent;
                    if (p is TableLayoutPanel tlp)
                    {
                        tlp.SetRowSpan(result, RowSpan);
                        tlp.SetColumnSpan(result, ColumnSpan);
                    }
                });
            }

            if (ForeColor.HasValue)
                result.ForeColor = ForeColor.Value;
            if (BackColor.HasValue)
                result.BackColor = BackColor.Value;

            return result;
        }

        protected Control[] BuildChildren(FluentState state) =>
            Controls?.Select(c => c?.Build(state)).Where(c => c != null).ToArray();

        protected override void SetDefault(Control control)
        {
            base.SetDefault(control);

            // MSDN:
            // The Anchor and Dock properties are mutually exclusive.
            // Only one can be set at a time, and the last one set takes precedence.
            if (Anchor != AnchorStyles.None)
                control.Anchor = Anchor;
            else if (Dock != DockStyle.None)
                control.Dock = Dock;

            control.Padding = Padding;

            if (Font != null)
                control.Font = Font;

            if (MinimumSize != Size.Empty)
                control.MinimumSize = MinimumSize;

            if (MaximumSize != Size.Empty)
                control.MaximumSize = MaximumSize;
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

        public override Control Build(FluentState state, Control parent = null)
        {
            var result = base.Build(state, parent);
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
        public TValue InitialValue { get; set; }
        public Action<TControl, TValue> OnChange { get; set; }
    }
}