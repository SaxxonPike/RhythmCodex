using System.Linq;
using RhythmCodex.Arc.Model;
using RhythmCodex.Compression;
using RhythmCodex.IoC;

namespace RhythmCodex.Arc.Converters
{
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
            var data = file.CompressedSize == file.DecompressedSize
                ? file.Data.ToArray()
                : _arcLzDecoder.Decode(file.Data);
            
            return new ArcFile
            {
                Name = file.Name,
                DecompressedSize = data.Length,
                CompressedSize = data.Length,
                Data = data
            };
        }
    }
}