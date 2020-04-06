using System.IO;

namespace RhythmCodex.Ddr.Streamers
{
    public interface IDdrPs2FileDataTableStreamReader
    {
        byte[] Get(Stream stream);
    }
}