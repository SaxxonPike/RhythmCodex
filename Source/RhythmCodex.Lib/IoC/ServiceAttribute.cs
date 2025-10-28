using System;
using JetBrains.Annotations;

namespace RhythmCodex.IoC;

/// <summary>
///     Marks a particular class as a service.
/// </summary>
[MeansImplicitUse(ImplicitUseTargetFlags.WithInheritors)]
[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(bool singleInstance = true) : Attribute
{
    public bool SingleInstance => singleInstance;
}