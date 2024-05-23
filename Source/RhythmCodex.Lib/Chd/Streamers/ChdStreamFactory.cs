using System.IO;
using RhythmCodex.Flac.Converters;
using RhythmCodex.IoC;
using RhythmCodex.Lzma.Converters;

namespace RhythmCodex.Chd.Streamers;

[Service]
public class ChdStreamFactory : IChdStreamFactory
{
    private readonly IFlacDecoder _flacDecoder;
    private readonly ILzmaDecoder _lzmaDecoder;

    public ChdStreamFactory(IFlacDecoder flacDecoder, ILzmaDecoder lzmaDecoder)
    {
        _flacDecoder = flacDecoder;
        _lzmaDecoder = lzmaDecoder;
    }
        
    public ChdStream Create(Stream source) => new(_flacDecoder, _lzmaDecoder, source);
        
    public ChdStream Create(Stream source, ChdStream parent) => new(_flacDecoder, _lzmaDecoder, source, parent);
}