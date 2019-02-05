using System.IO;
using System.Linq;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Beatmania.Streamers
{
    [Service]
    public class BeatmaniaPs2NewBgmStreamReader : IBeatmaniaPs2NewBgmStreamReader
    {
        private readonly IVagStreamReader _vagStreamReader;

        public BeatmaniaPs2NewBgmStreamReader(IVagStreamReader vagStreamReader)
        {
            _vagStreamReader = vagStreamReader;
        }
        
        public BeatmaniaPs2Bgm Read(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var header = Enumerable.Range(0, 12).Select(i => reader.ReadInt32()).ToArray();

            var headerLength = header[2];
            var dataLength = header[3];
            var loopStart = header[4];
            var loopEnd = header[5];
            var rate = header[6];
            var channels = header[7];
            var interleave = header[9];
            var volume = header[10];

            reader.ReadBytes(headerLength - 0x30);

            var data = reader.ReadBytes(dataLength);
            using (var mem = new MemoryStream(data))
            {
                return new BeatmaniaPs2Bgm
                {
                    Channels = channels,
                    Rate = rate,
                    Volume = volume,
                    Data = _vagStreamReader.Read(mem, channels, interleave)
                };
            }
        }
    }
}