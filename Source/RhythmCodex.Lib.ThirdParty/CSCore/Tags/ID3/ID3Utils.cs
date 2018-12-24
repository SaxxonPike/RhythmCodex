﻿using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;

namespace CSCore.Tags.ID3
{
    internal static class ID3Utils
    {
        public static readonly Encoding Iso88591 = Encoding.GetEncoding("ISO-8859-1");
        public static readonly Encoding Utf16 = new UnicodeEncoding(false, true);
        public static readonly Encoding Utf16Big = new UnicodeEncoding(true, true);
        public static readonly Encoding Utf8 = new UTF8Encoding();

        public static unsafe Int32 ReadInt32(byte[] array, int offset, bool sync, int length = 4)
        {
            /*fixed (byte* ptr = array)
            {
                byte* p = ptr + offset;
                return p[0] * (1 << 21) + p[1] * (1 << 14) + p[2] * (1 << 7) + p[3];
            }*/
            var value = 0;

            for (var i = offset; i < offset + length; i++)
            {
                if (sync)
                {
                    if ((array[i] & 0x80) == 0x80)
                        throw new ID3Exception("Unknown error");
                    value = (value << 7) | array[i];
                }
                else
                {
                    value = (value << 8) | array[i];
                }
            }

            return Math.Max(value, 0);
        }

        public static Int32 ReadInt32(Stream stream, bool sync, int length = 4)
        {
            var buffer = new byte[4];
            if (stream.Read(buffer, 0, buffer.Length) < buffer.Length)
                throw new EndOfStreamException();

            return ReadInt32(buffer, 0, sync, length);
        }

        public static byte[] Read(Stream stream, int count)
        {
            var buffer = new byte[count];
            if (stream.Read(buffer, 0, count) < count)
                throw new EndOfStreamException();
            return buffer;
        }

        public static string ReadString(byte[] buffer, int offset, int count, Encoding encoding)
        {
            int read;
            return ReadString(buffer, offset, count, encoding, out read);
        }

        public static string ReadString(byte[] buffer, int offset, int count, Encoding encoding, out int read)
        {
            var sizeofsymbol = (encoding == Utf16 || encoding == Utf16Big) ? 2 : 1;

            if (count == -1)
                count = buffer.Length;

            var index = SeekPreamble(buffer, offset, count, encoding);

            var length = CalculateStringLength(buffer, offset, count, sizeofsymbol);
            var result = encoding.GetString(buffer, offset, length);

            read = 0;
            read += (index - offset); //preamble
            read += length; //length of string itself
            read += sizeofsymbol; //escape

            return result;
        }

        public static Encoding GetEncoding(byte[] buffer, int offset, int stringOffset)
        {
            var encodingByte = buffer[offset];

            if (encodingByte == 0)
                return Encoding.Default;
            if (encodingByte == 1)
            {
                if (buffer.Length < stringOffset + 2)
                    throw new ArgumentException("buffer to small");

                if (buffer[stringOffset] == 0xFE && buffer[stringOffset + 1] == 0xFF)
                    return Utf16Big;
                else if (buffer[stringOffset] == 0xFF && buffer[stringOffset + 1] == 0xFE)
                    return Utf16;
                else throw new ID3Exception("Can't detected UTF encoding");
            }
            else if (encodingByte == 2)
                return Utf16Big;
            else if (encodingByte == 3)
                return Utf8;
            else throw new ID3Exception("Invalid Encodingbyte");
        }

        private static int CalculateStringLength(byte[] buffer, int offset, int count, int sizeofsymbol)
        {
            var index = offset;
            var symcount = sizeofsymbol - 1;

            while (index < Math.Min(buffer.Length, count + offset) && (buffer[index] != 0 || buffer[index + symcount] != 0))
            {
                index += sizeofsymbol;
            }

            return index - offset;
        }

        private static int SeekPreamble(byte[] buffer, int offset, int count, Encoding e)
        {
            var prem = e.GetPreamble();
            if (prem.Length + offset > buffer.Length ||
               prem.Length > count)
                return offset;

            var newoffset = 0;
            while (newoffset < prem.Length && prem[newoffset] == buffer[newoffset + offset])
                newoffset++;

            if (newoffset == prem.Length)
                return offset + newoffset;
            else return offset;
        }

        public const string MimeURL = "-->";

        public static Image DecodeImage(byte[] rawdata, string mimetype)
        {
            Stream stream;
            if (mimetype.Trim() == MimeURL)
            {
                var client = new WebClient();
                var data = client.DownloadData(GetURL(rawdata, mimetype));
                stream = new MemoryStream(data);
            }
            else
            {
                stream = new MemoryStream(rawdata, false);
            }

            return Image.FromStream(stream);
        }

        public static string GetURL(byte[] RawData, string MimeType)
        {
            if (RawData == null)
                throw new InvalidOperationException("Decode the frame first");
            if (MimeType != MimeURL)
                throw new InvalidOperationException("MimeType != " + MimeURL);

            return ReadString(RawData, 0, -1, Iso88591);
        }
    }
}