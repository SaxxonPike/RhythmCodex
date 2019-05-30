using System.IO;

namespace RhythmCodex.Digital573.Streamers
{
    public interface IDigital573AudioStreamReader
    {
        byte[] Read(Stream stream, long length);
    }
}