using JetBrains.Annotations;

namespace RhythmCodex;

[PublicAPI]
public static class CharacterSets
{
    public const string LowercaseAsciiLetters = "abcdefghijklmnopqrstuvwxyz";
    public const string UppercaseAsciiLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string AsciiLetters = LowercaseAsciiLetters + UppercaseAsciiLetters;
    public const string AsciiNumbers = "0123456789";
    public const string AsciiLettersAndNumbers = AsciiLetters + AsciiNumbers;
    public const string Hexidecimal = AsciiNumbers + "ABCDEF";
    public const string HexidecimalWithLowercaseLetters = Hexidecimal + "abcdef";
}