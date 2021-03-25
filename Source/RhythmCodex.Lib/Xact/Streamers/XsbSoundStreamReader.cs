using System;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XsbSoundStreamReader : IXsbSoundStreamReader
    {
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

            sound.Clips = new XsbSoundClip[isComplex ? reader.ReadByte() : 0];
            sound.TrackIndex = isComplex ? (short) 0 : reader.ReadInt16();
            sound.WaveBankIndex = isComplex ? (byte) 0 : reader.ReadByte();

            if (!hasRpc)
            {
                sound.RpcData = Array.Empty<byte>();
            }
            else
            {
                var length = reader.ReadInt16();
                if (length >= 2)
                    sound.RpcData = reader.ReadBytes(length - 2);
            }

            if (!hasDsp)
            {
                sound.DspData = Array.Empty<byte>();
            }
            else
            {
                var length = reader.ReadInt16();
                if (length >= 2)
                    sound.DspData = reader.ReadBytes(length - 2);
            }

            if (isComplex)
            {
                // load clips here- I'm not doing this right now.
            }

            return sound;
        }
    }
}