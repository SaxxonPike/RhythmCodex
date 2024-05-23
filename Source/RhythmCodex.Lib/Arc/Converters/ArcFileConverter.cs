using System.IO;
using System.Linq;
using RhythmCodex.Arc.Model;
using RhythmCodex.Compression;
using RhythmCodex.IoC;

namespace RhythmCodex.Arc.Converters;

/// <inheritdoc />
[Service]
public class ArcFileConverter : IArcFileConverter
{
    private readonly IArcLzDecoder _arcLzDecoder;
    private readonly IArcLzEncoder _arcLzEncoder;

    public ArcFileConverter(IArcLzDecoder arcLzDecoder, IArcLzEncoder arcLzEncoder)
    {
        _arcLzDecoder = arcLzDecoder;
        _arcLzEncoder = arcLzEncoder;
    }

    public ArcFile Compress(ArcFile file)
    {
        var data = file.CompressedSize != file.DecompressedSize
            ? file.Data.ToArray()
            : _arcLzEncoder.Encode(file.Data);

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
        byte[] data;

        if (file.CompressedSize == file.DecompressedSize)
        {
            data = file.Data.ToArray();
        }
        else
        {
            using var stream = new MemoryStream(file.Data);
            data = _arcLzDecoder.Decode(stream);
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