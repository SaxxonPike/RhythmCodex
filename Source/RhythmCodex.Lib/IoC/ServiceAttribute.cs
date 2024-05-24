using System;
using JetBrains.Annotations;

namespace RhythmCodex.IoC;

/// <summary>
///     Marks a particular class as a service.
/// </summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(bool singleInstance = true) : Attribute
{
    public bool SingleInstance { get; } = singleInstance;
}