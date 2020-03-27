using System.IO;

namespace RhythmCodex.Ddr.Streamers
{
    public interface IDdrPs2FileDataStepStreamReader
    {
        byte[] Read(Stream fileDataBinStream, long length);
    }
}