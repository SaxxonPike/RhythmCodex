using System;

namespace RhythmCodex.Infrastructure
{
    /// <summary>
    ///     Marks a particular class as something that should be intantiated every time it is requested.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InstancePerDependencyAttribute : Attribute
    {
    }
}