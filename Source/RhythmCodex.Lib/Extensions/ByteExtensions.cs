using System.Diagnostics;

namespace RhythmCodex.Extensions;

[DebuggerStepThrough]
internal static class ByteExtensions
{
    extension(byte value)
    {
        public bool IsLetter() => 
            value is >= 0x41 and <= 0x5A 
                or >= 0x61 and <= 0x7A;

        public bool IsLetterOrDigit() => 
            value is >= 0x41 and <= 0x5A 
                or >= 0x61 and <= 0x7A 
                or >= 0x30 and <= 0x39;
    }
}