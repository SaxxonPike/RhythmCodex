using System;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Iso.Converters;

[Service]
public class IsoDateTimeDecoder : IIsoDateTimeDecoder
{
    public DateTimeOffset? Decode(ReadOnlySpan<byte> data)
    {
        var year = data[0] + 1900;
        var month = data[1];
        var day = data[2];
        var hour = data[3];
        var minute = data[4];
        var second = data[5];
        var offset = data[6];

        try
        {
            return new DateTimeOffset(
                year,
                month,
                day,
                hour,
                minute,
                second,
                TimeSpan.FromMinutes((offset - 48) * 15));
        }
        catch (Exception)
        {
            return null;
        }
    }
}