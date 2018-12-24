using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;

namespace RhythmCodex.ThirdParty
{
    [Service]
    public class Mp3Adapter : IMp3Adapter
    {
        public ISound Decode(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}