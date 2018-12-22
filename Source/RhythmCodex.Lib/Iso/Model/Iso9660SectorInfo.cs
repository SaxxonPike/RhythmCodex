using System;
using System.Diagnostics;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Iso.Model
{
    [DebuggerStepThrough]
    [Model]
    public class Iso9660SectorInfo
    {
        public int Number { get; set; }
        public byte[] Data { get; set; }
        public int UserDataOffset { get; set; }
        public int UserDataLength { get; set; }
        public int? EdcOffset { get; set; }
        public int? EccOffset { get; set; }
        public int? Minutes { get; set; }
        public int? Seconds { get; set; }
        public int? Frames { get; set; }
        public int? Mode { get; set; }
        public int? Form { get; set; }
        public int? Id { get; set; }
        public int? Channel { get; set; }
        public bool? EndOfFile { get; set; }
        public bool? IsTimeDependent { get; set; }
        public bool? Trigger { get; set; }
        public bool? IsData { get; set; }
        public bool? IsAudio { get; set; }
        public bool? IsVideo { get; set; }
        public bool? EndOfRecord { get; set; }
        public int? AudioChannels { get; set; }
        public int? AudioRate { get; set; }
        public int? AudioBitsPerSample { get; set; }
        public bool? AudioEmphasis { get; set; }

        public Span<byte> UserData => Data.AsSpan(UserDataOffset, UserDataLength);

        public override string ToString()
        {
            return $"Mode={Mode} Time={Minutes}:{Seconds}:{Frames}";
        }
    }
}