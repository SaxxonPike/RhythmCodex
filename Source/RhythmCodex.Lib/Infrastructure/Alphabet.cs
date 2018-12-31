using System;
using System.Text;

namespace RhythmCodex.Infrastructure
{
    public static class Alphabet
    {
        private const string Hex = "0123456789ABCDEF";
        private const string Alphanumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numeric = "0123456789";

        private static string Encode(string dict, int value, int length)
        {
            if (value < 0)
                throw new ArgumentException("Value to encode must be zero or greater.", nameof(value));
            
            var dictLength = dict.Length;
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                builder.Insert(0, dict[value % dictLength]);
                value /= dictLength;
            }

            return builder.ToString();
        }

        public static string EncodeHex(int value, int length) => Encode(Hex, value, length);
        public static string EncodeNumeric(int value, int length) => Encode(Numeric, value, length);
        public static string EncodeAlphanumeric(int value, int length) => Encode(Alphanumeric, value, length);

        private static int Decode(string dict, ReadOnlySpan<char> value)
        {
            var output = 0;
            var dictLength = dict.Length;
            var valueLength = value.Length;
            
            for (var i = 0; i < valueLength; i++)
            {
                output *= dictLength;
                var val = dict.IndexOf(value[i]);
                if (val > 0)
                    output += val;
            }

            return output;
        }

        public static int DecodeHex(ReadOnlySpan<char> value) => Decode(Hex, value);
        public static int DecodeNumeric(ReadOnlySpan<char> value) => Decode(Numeric, value);
        public static int DecodeAlphanumeric(ReadOnlySpan<char> value) => Decode(Alphanumeric, value);
    }
}