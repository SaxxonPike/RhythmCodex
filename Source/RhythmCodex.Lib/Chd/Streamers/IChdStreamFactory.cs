using System.IO;

namespace RhythmCodex.Chd.Streamers
{
    public interface IChdStreamFactory
    {
        ChdStream Create(Stream source);
        ChdStream Create(Stream source, ChdStream parent);
    }
}