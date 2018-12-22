using System.IO;
using RhythmCodex.Infrastructure;
using RhythmCodex.Vag.Models;

namespace RhythmCodex.Vag.Streamers
{
    [Service]
    public class SvagStreamReader : ISvagStreamReader
    {
        private readonly IVagStreamReader _vagStreamReader;

        public SvagStreamReader(IVagStreamReader vagStreamReader)
        {
            _vagStreamReader = vagStreamReader;
        }
        
        public SvagContainer Read(Stream stream)
        {
            var reader = new BinaryReader(stream);
            if (reader.ReadInt32() != 0x67617653)
                throw new RhythmCodexException("Svag header ID is not valid.");
            reader.ReadInt32(); // data length
            var freq = reader.ReadInt32();
            var channels = reader.ReadInt32();
            var interleave = reader.ReadInt32();
            reader.ReadInt32(); // reserved 0
            reader.ReadInt32(); // reserved 1
            reader.ReadBytes(0x800 - 0x1C); // discard the rest of the header

            var chunk = _vagStreamReader.Read(stream, channels, interleave);
            return new SvagContainer
            {
                VagChunk = chunk,
                SampleRate = freq
            };
        }
    }
}