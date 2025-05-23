using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Model;

[Model]
public record BmsCommand
{
    public string? Name { get; set; }
    public string? Value { get; set; }
    public bool UseColon { get; set; }
    public string? Comment { get; set; }

    public override string ToString()
    {
        if (Name != null)
        {
            return Value != null 
                ? $"#{Name}{(UseColon ? ":" : " ")}{Value} {Comment}" 
                : $"#{Name} {Comment}";
        }

        return $"{Comment}";
    }
}