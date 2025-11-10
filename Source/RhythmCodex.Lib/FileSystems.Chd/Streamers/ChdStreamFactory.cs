using System.IO;
using RhythmCodex.Compressions.Lzma.Converters;
using RhythmCodex.IoC;

namespace RhythmCodex.FileSystems.Chd.Streamers;

[Service]
public class ChdStreamFactory(ILzmaDecoder lzmaDecoder) : IChdStreamFactory
{
    public ChdStream Create(Stream source) => new(lzmaDecoder, source);
        
    public ChdStream Create(Stream source, ChdStream parent) => new(lzmaDecoder, source, parent);
}