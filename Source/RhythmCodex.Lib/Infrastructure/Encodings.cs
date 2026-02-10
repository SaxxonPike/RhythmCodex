using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RhythmCodex.Infrastructure;

[DebuggerStepThrough]
public static class Encodings
{
    public static readonly Encoding Cp437 = CodePagesEncodingProvider.Instance.GetEncoding(437)!;
    public static readonly Encoding Cp932 = CodePagesEncodingProvider.Instance.GetEncoding(932)!;
    public static readonly Encoding Cp1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252)!;
    public static readonly Encoding Utf8 = Encoding.UTF8;
    public static readonly Encoding Ascii = Encoding.ASCII;

    extension(Encoding encoding)
    {
        public string GetString(ReadOnlySpan<byte> bytes) =>
            encoding.GetString(bytes.ToArray());

        public string GetStringWithoutNulls(ReadOnlySpan<byte> bytes)
        {
            var input = new List<byte>();
            var length = bytes.Length;
            var offset = 0;
            while (offset < length)
            {
                var b = bytes[offset++];
                if (b == 0)
                    break;
                input.Add(b);
            }
            return encoding.GetString(input.ToArray());
        }
    }
}