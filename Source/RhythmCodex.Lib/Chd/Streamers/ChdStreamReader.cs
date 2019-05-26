using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.ThirdParty;

namespace RhythmCodex.Chd.Streamers
{
    [Service]
    public class ChdStreamFactory : IChdStreamFactory
    {
        private readonly IFlacDecoder _flacDecoder;

        public ChdStreamFactory(IFlacDecoder flacDecoder)
        {
            _flacDecoder = flacDecoder;
        }
        
        public ChdStream Create(Stream source) => 
            new ChdStream(_flacDecoder, source);
        
        public ChdStream Create(Stream source, ChdStream parent) =>
            new ChdStream(_flacDecoder, source, parent);
    }
}