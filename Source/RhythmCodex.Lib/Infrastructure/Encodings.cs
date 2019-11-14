using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RhythmCodex.Infrastructure
{
    [DebuggerStepThrough]
    public static class Encodings
    {
        public static readonly Encoding CP437 = CodePagesEncodingProvider.Instance.GetEncoding(437);
        public static readonly Encoding CP932 = CodePagesEncodingProvider.Instance.GetEncoding(932);
        public static readonly Encoding CP1252 = CodePagesEncodingProvider.Instance.GetEncoding(1252);
        public static readonly Encoding UTF8 = Encoding.UTF8;

        public static string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes) =>
            encoding.GetString(bytes.ToArray());

        public static string GetStringWithoutNulls(this Encoding encoding, ReadOnlySpan<byte> bytes)
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