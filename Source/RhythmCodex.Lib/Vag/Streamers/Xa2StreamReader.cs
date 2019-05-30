using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Streamers
{
    [Service]
    public class Xa2StreamReader : IXa2StreamReader
    {
        private readonly IVagStreamReader _vagStreamReader;

        public Xa2StreamReader(IVagStreamReader vagStreamReader)
        {
            _vagStreamReader = vagStreamReader;
        }
        
        public Xa2Container Read(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var channels = reader.ReadInt32();
            var interleave = reader.ReadInt32();
            reader.ReadBytes(0x800 - 0x008); // discard the rest of the header

            var chunk = _vagStreamReader.Read(stream, channels, interleave);
            return new Xa2Container
            {
                VagChunk = chunk
            };
        }
    }
}