using System.IO;
using RhythmCodex.IoC;

namespace RhythmCodex.Chd.Streamers
{
    [Service]
    public class ChdStreamFactory : IChdStreamFactory
    {
        public ChdStream Create(Stream source) => 
            new ChdStream(source);
        
        public ChdStream Create(Stream source, ChdStream parent) =>
            new ChdStream(source, parent);
    }
}