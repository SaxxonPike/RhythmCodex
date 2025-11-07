using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Models;

namespace RhythmCodex.Sounds.Vag.Streamers;

[Service]
public class Xa2StreamReader(IVagStreamReader vagStreamReader) : IXa2StreamReader
{
    public Xa2Container Read(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var channels = reader.ReadInt32();
        var interleave = reader.ReadInt32();
        reader.ReadBytes(0x800 - 0x008); // discard the rest of the header

        var chunk = vagStreamReader.Read(stream, channels, interleave);
        return new Xa2Container
        {
            VagChunk = chunk
        };
    }
}