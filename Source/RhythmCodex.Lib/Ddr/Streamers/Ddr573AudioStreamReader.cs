using System;
using System.IO;
using System.Linq;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Streamers
{
    [Service]
    public class Ddr573AudioStreamReader : IDdr573AudioStreamReader
    {
        private readonly IDdr573AudioDecrypter _ddr573AudioDecrypter;
//        private const int KeySize = 0x1E00;

        public Ddr573AudioStreamReader(IDdr573AudioDecrypter ddr573AudioDecrypter)
        {
            _ddr573AudioDecrypter = ddr573AudioDecrypter;
        }

        public byte[] Read(Stream stream, long length)
        {
            throw new NotImplementedException();
        }
    }
}