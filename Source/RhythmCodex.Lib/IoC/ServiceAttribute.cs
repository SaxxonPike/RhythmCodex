using System;
using JetBrains.Annotations;

namespace RhythmCodex.IoC;

/// <summary>
///     Marks a particular class as a service.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
    public ServiceAttribute(bool singleInstance = true)
    {
        SingleInstance = singleInstance;
    }

    public bool SingleInstance { get; }
}