using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.ThirdParty
{
    [Service]
    public class OggAdapter : IOggAdapter
    {
        public ISound Decode(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}