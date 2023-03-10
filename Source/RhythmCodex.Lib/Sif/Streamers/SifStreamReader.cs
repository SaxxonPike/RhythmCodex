using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sif.Converters;
using RhythmCodex.Sif.Models;

namespace RhythmCodex.Sif.Streamers
{
    [Service]
    public class SifStreamReader : ISifStreamReader
    {
        private readonly IBinarySifDecoder _binarySifDecoder;
        private readonly ITextSifDecoder _textSifDecoder;

        public SifStreamReader(
            IBinarySifDecoder binarySifDecoder,
            ITextSifDecoder textSifDecoder)
        {
            _binarySifDecoder = binarySifDecoder;
            _textSifDecoder = textSifDecoder;
        }
        
        public SifInfo Read(Stream stream, long length)
        {
            var data = new BinaryReader(stream).ReadBytes((int) length);
            return data[0] == 0x00 
                ? _binarySifDecoder.Decode(data) 
                : ReadTextSif(data);
        }

        private SifInfo ReadTextSif(byte[] data)
        {
            using var dataStream = new ReadOnlyMemoryStream(data);
            return _textSifDecoder.Decode(dataStream.ReadAllLines());
        }
    }
}