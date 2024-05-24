using System.Collections.Generic;
using System.IO;
using RhythmCodex.Graphics.Models;
using RhythmCodex.IoC;
using RhythmCodex.Tim.Streamers;

namespace RhythmCodex.Tim.Converters;

[Service]
public class TimDecoder(
    ITimBitmapDecoder bitmapDecoder,
    ITimStreamReader streamReader)
    : ITimDecoder
{
    public List<Bitmap> Decode(Stream stream)
    {
        var image = streamReader.Read(stream);
        return bitmapDecoder.Decode(image);
    }
}