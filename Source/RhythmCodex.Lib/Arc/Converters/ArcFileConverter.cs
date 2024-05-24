using System;
using RhythmCodex.Arc.Model;
using RhythmCodex.Compression;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Arc.Converters;

/// <inheritdoc />
[Service]
public class ArcFileConverter(IArcLzDecoder arcLzDecoder, IArcLzEncoder arcLzEncoder) : IArcFileConverter
{
    public ArcFile Compress(ArcFile file)
    {
        var data = file.CompressedSize != file.DecompressedSize
            ? file.Data
            : arcLzEncoder.Encode(file.Data.Span);

        return new ArcFile
        {
            Name = file.Name,
            DecompressedSize = file.DecompressedSize,
            CompressedSize = data.Length,
            Data = data
        };
    }

    public ArcFile Decompress(ArcFile file)
    {
        Memory<byte> data;

        if (file.CompressedSize == file.DecompressedSize)
        {
            data = file.Data;
        }
        else
        {
            using var stream = new ReadOnlyMemoryStream(file.Data);
            data = arcLzDecoder.Decode(stream);
        }

        return new ArcFile
        {
            Name = file.Name,
            DecompressedSize = data.Length,
            CompressedSize = data.Length,
            Data = data
        };
    }
}