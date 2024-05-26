using System;

namespace RhythmCodex.Infrastructure;

/// <summary>
///     Marks a class or struct as a data model.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ModelAttribute : Attribute;