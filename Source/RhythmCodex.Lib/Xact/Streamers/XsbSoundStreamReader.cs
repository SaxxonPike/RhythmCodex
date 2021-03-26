using System;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XsbSoundStreamReader : IXsbSoundStreamReader
    {
        private readonly IXsbSoundDspStreamReader _xsbSoundDspStreamReader;
        private readonly IXsbSoundRpcStreamReader _xsbSoundRpcStreamReader;
        private readonly IXsbSoundClipStreamReader _xsbSoundClipStreamReader;

        public XsbSoundStreamReader(
            IXsbSoundDspStreamReader xsbSoundDspStreamReader,
            IXsbSoundRpcStreamReader xsbSoundRpcStreamReader,
            IXsbSoundClipStreamReader xsbSoundClipStreamReader)
        {
            _xsbSoundDspStreamReader = xsbSoundDspStreamReader;
            _xsbSoundRpcStreamReader = xsbSoundRpcStreamReader;
            _xsbSoundClipStreamReader = xsbSoundClipStreamReader;
        }
        
        public XsbSound Read(Stream stream)
        {
            var reader = new BinaryReader(stream);

            var sound = new XsbSound {Flags = reader.ReadByte()};

            var isComplex = (sound.Flags & 0x01) != 0;
            var hasRpc = (sound.Flags & 0x0E) != 0;
            var hasDsp = (sound.Flags & 0x10) != 0;

            sound.Category = reader.ReadInt16();
            sound.Volume = reader.ReadByte();
            sound.Pitch = reader.ReadInt16();
            sound.Priority = reader.ReadByte();
            sound.Unk0 = reader.ReadInt16();

            var numClips = isComplex ? reader.ReadByte() : 0;
            sound.TrackIndex = isComplex ? (short) 0 : reader.ReadInt16();
            sound.WaveBankIndex = isComplex ? (byte) 0 : reader.ReadByte();

            if (hasRpc)
                sound.Rpc = _xsbSoundRpcStreamReader.Read(stream);

            if (hasDsp)
                sound.Dsp = _xsbSoundDspStreamReader.Read(stream);

            if (isComplex)
                sound.Clips = _xsbSoundClipStreamReader.Read(stream, numClips);

            return sound;
        }
    }
}