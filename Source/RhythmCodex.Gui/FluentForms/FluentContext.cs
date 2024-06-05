using System;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms;

public class FluentContext(FluentState state)
{
    public TControl? GetControl<TControl>(string id) 
        where TControl : class
    {
        if (!state.Map.TryGetValue(id, out var obj))
            return null;
        return obj as TControl;
    }
}
    
public class FluentContext<TFluent, TSelf>(TFluent blueprint, TSelf control, FluentState state) : FluentContext(state)
{
    public TSelf Control { get; } = control;
    public TFluent Blueprint { get; } = blueprint;

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
            return control.Invoke(del);
        }

        return del();
    }
}