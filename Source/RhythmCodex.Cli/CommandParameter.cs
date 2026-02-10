namespace RhythmCodex.Cli;

/// <inheritdoc />
public record CommandParameter : ICommandParameter
{
    /// <inheritdoc />
    public required string Name { get; init; }
    /// <inheritdoc />
    public required string Description { get; init; }
}