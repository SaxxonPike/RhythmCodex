using System;

namespace RhythmCodex.IoC
{
    /// <summary>
    ///     Marks a particular class as a service.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(bool singleInstance = true)
        {
            SingleInstance = singleInstance;
        }

        public bool SingleInstance { get; }
    }
}