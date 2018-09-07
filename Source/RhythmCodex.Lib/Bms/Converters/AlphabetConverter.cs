using System;
using System.Text;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Bms.Converters
{
    [Service]
    public class AlphabetConverter : IAlphabetConverter
    {
        private const string Hex = "0123456789ABCDEF";
        private const string Alphanumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Numeric = "0123456789";

        private string Encode(string dict, int value, int length)
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

        public string EncodeHex(int value, int length) => Encode(Hex, value, length);
        public string EncodeNumeric(int value, int length) => Encode(Numeric, value, length);
        public string EncodeAlphanumeric(int value, int length) => Encode(Alphanumeric, value, length);
    }
}