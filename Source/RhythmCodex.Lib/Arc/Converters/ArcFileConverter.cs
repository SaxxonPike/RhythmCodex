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
            return new ArcFile
            {
                Name = file.Name,
                IsCompressed = true,
                Data = file.IsCompressed
                    ? file.Data.ToArray()
                    : _arcLzEncoder.Encode(file.Data)
            };
        }

        public ArcFile Decompress(ArcFile file)
        {
            return new ArcFile
            {
                Name = file.Name,
                IsCompressed = false,
                Data = file.IsCompressed
                    ? _arcLzDecoder.Decode(file.Data)
                    : file.Data.ToArray()
            };
        }
    }
}