using System.IO;
using RhythmCodex.Compressions.Lzma.Converters;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Flac.Converters;

namespace RhythmCodex.FileSystems.Chd.Streamers;

[Service]
public class ChdStreamFactory(IFlacDecoder flacDecoder, ILzmaDecoder lzmaDecoder) : IChdStreamFactory
{
    public ChdStream Create(Stream source) => new(flacDecoder, lzmaDecoder, source);
        
    public ChdStream Create(Stream source, ChdStream parent) => new(flacDecoder, lzmaDecoder, source, parent);
}