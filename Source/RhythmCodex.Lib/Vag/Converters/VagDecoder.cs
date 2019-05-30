using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Converters
{
    [Service]
    public class VagDecoder : IVagDecoder
    {
        private readonly IVagSplitter _vagSplitter;

        public VagDecoder(IVagSplitter vagSplitter)
        {
            _vagSplitter = vagSplitter;
        }
        
        public ISound Decode(VagChunk chunk)
        {
            return new Sound
            {
                Samples = _vagSplitter.Split(chunk)
            };
        }
    }
}