using System.IO;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Beatmania.Streamers
{
    [Service]
    public class BeatmaniaPcAudioEntryStreamReader : IBeatmaniaPcAudioEntryStreamReader
    {
        public BeatmaniaPcAudioEntry Read(Stream source)
        {
            var reader = new BinaryReader(source);
            if (new string(reader.ReadChars(4)) != "2DX9")
                throw new RhythmCodexException("Unexpected audio entry header.");

            var infoLength = reader.ReadInt32();
            var dataLength = reader.ReadInt32();
            var reserved = reader.ReadInt16();
            int channel = reader.ReadInt16();
            int panning = reader.ReadInt16();
            int volume = reader.ReadInt16();
            var extraInfo = reader.ReadBytes(infoLength - 0x14);

            var wavData = reader.ReadBytes(dataLength);
            
            return new BeatmaniaPcAudioEntry
            {
                Channel = channel,
                Data = wavData,
                ExtraInfo = extraInfo,
                Panning = panning,
                Reserved = reserved,
                Volume = volume
            };
        }
    }
}