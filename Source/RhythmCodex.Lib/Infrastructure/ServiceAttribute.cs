using System;

namespace RhythmCodex.Infrastructure
{
    /// <summary>
    /// Marks a particular class as a service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
    }
}