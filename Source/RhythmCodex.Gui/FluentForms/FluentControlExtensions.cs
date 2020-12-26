using System;
using System.Windows.Forms;

namespace RhythmCodex.Gui.FluentForms
{
    public static class FluentControlExtensions
    {
        public static Control Build(this FluentControl fc)
        {
            var state = new FluentState();
            var result = fc.Build(state);
            foreach (var callback in state.Callbacks)
                callback();
            return result;
        }
        
        public static T With<T>(this T self, Action<T> act)
            where T : FluentComponent
        {
            act(self);
            return self;
        }
    }
}