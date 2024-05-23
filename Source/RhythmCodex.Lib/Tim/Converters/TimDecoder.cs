using System.Collections.Generic;
using System.IO;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;
using RhythmCodex.Tim.Streamers;

namespace RhythmCodex.Tim.Converters;

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
        
    public IList<IBitmap> Decode(Stream stream)
    {
        var image = _streamReader.Read(stream);
        return _bitmapDecoder.Decode(image);
    }
}