using System;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public class FluentContext
    {
        private readonly FluentState _state;

        public FluentContext(FluentState state)
        {
            _state = state;
        }
        
        public TControl GetControl<TControl>(string id) 
            where TControl : class
        {
            if (!_state.Map.TryGetValue(id, out var obj))
                return null;
            return obj as TControl;
        }
    }
    
    public class FluentContext<TFluent, TSelf> : FluentContext
    {
        public FluentContext(TFluent blueprint, TSelf control, FluentState state) : base(state)
        {
            Blueprint = blueprint;
            Control = control;
        }

        public TSelf Control { get; }
        public TFluent Blueprint { get; }
        
        public void Invoke(Action del)
        {
            if (Control is Control control)
            {
                control.Invoke(del);
            }
            else
            {
                del();
            }
        }

        public T Invoke<T>(Func<T> del)
        {
            if (Control is Control control)
            {
                return (T) control.Invoke(del);
            }

            return del();
        }
    }
}