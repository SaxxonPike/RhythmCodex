using System.Collections.Generic;
using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Tim.Streamers;

namespace RhythmCodex.Tim.Converters
{
    [Service]
    public class TimDecoder : ITimDecoder
    {
        private readonly ITimBitmapDecoder _bitmapDecoder;
        private readonly ITimStreamReader _streamReader;

        public TimDecoder(
            ITimBitmapDecoder bitmapDecoder, 
            ITimStreamReader streamReader)
        {
            _bitmapDecoder = bitmapDecoder;
            _streamReader = streamReader;
        }
        
        public IList<RawBitmap> Decode(Stream stream)
        {
            var image = _streamReader.Read(stream);
            return _bitmapDecoder.Decode(image);
        }
    }
}