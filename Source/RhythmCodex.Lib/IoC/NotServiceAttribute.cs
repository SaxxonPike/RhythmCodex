using System;

namespace RhythmCodex.IoC
{
    /// <summary>
    /// A reminder to NOT use the Service attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NotServiceAttribute : Attribute
    {
    }
}