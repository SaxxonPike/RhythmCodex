using System;
using System.Linq;

namespace RhythmCodex.IoC;

/// <summary>
/// A type map for a single service.
/// </summary>
/// <param name="Implementation">
/// Implementation of a service.
/// </param>
/// <param name="Services">
/// Interfaces that a service should implement. These should all be interfaces.
/// </param>
/// <param name="SingleInstance">
/// If true, this service should only have a single instance in the container.
/// </param>
public readonly record struct ServiceMapping(Type Implementation, Type[] Services, bool SingleInstance)
{
    public override string ToString() => 
        $"{Implementation.FullName} <- {string.Join(", ", Services.Select(s => s.FullName))}";
}