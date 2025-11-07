using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Xact.Model;

namespace RhythmCodex.Sounds.Xact.Streamers;

[Service]
public class XsbSoundStreamReader(
    IXsbSoundDspStreamReader xsbSoundDspStreamReader,
    IXsbSoundRpcStreamReader xsbSoundRpcStreamReader,
    IXsbSoundClipStreamReader xsbSoundClipStreamReader)
    : IXsbSoundStreamReader
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

        var numClips = isComplex ? reader.ReadByte() : 0;
        sound.TrackIndex = isComplex ? (short) 0 : reader.ReadInt16();
        sound.WaveBankIndex = isComplex ? (byte) 0 : reader.ReadByte();

        if (hasRpc)
            sound.Rpc = xsbSoundRpcStreamReader.Read(stream);

        if (hasDsp)
            sound.Dsp = xsbSoundDspStreamReader.Read(stream);

        if (isComplex)
        {
            sound.Clips = new XsbSoundClip[numClips];
            for (var i = 0; i < numClips; i++)
                sound.Clips[i] = xsbSoundClipStreamReader.Read(stream);
        }

        return sound;
    }
}