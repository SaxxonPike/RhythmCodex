using System;
using System.IO;
using RhythmCodex.Digital573.Converters;
using RhythmCodex.IoC;

namespace RhythmCodex.Digital573.Streamers
{
    [Service]
    public class Digital573AudioStreamReader : IDigital573AudioStreamReader
    {
        private readonly IDigital573AudioDecrypter _digital573AudioDecrypter;
//        private const int KeySize = 0x1E00;

        public Digital573AudioStreamReader(IDigital573AudioDecrypter digital573AudioDecrypter)
        {
            _digital573AudioDecrypter = digital573AudioDecrypter;
        }

        public byte[] Read(Stream stream, long length)
        {
            throw new NotImplementedException();
        }
    }
}