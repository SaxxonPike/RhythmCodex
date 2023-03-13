namespace RhythmCodex.Extensions;

internal static class ByteExtensions
{
    internal static bool IsLetter(this byte value)
    {
        return (value >= 0x41 && value <= 0x5A) ||
               (value >= 0x61 && value <= 0x7A);
    }

    internal static bool IsLetterOrDigit(this byte value)
    {
        return (value >= 0x41 && value <= 0x5A) ||
               (value >= 0x61 && value <= 0x7A) ||
               (value >= 0x30 && value <= 0x39);
    }
}