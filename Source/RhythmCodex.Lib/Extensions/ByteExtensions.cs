using System.Diagnostics;

namespace RhythmCodex.Extensions;

[DebuggerStepThrough]
internal static class ByteExtensions
{
    public static bool IsLetter(this byte value) => 
        value is >= 0x41 and <= 0x5A 
            or >= 0x61 and <= 0x7A;

    public static bool IsLetterOrDigit(this byte value) => 
        value is >= 0x41 and <= 0x5A 
            or >= 0x61 and <= 0x7A 
            or >= 0x30 and <= 0x39;
}