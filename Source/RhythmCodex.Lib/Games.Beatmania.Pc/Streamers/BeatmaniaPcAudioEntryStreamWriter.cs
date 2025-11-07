using System.IO;
using RhythmCodex.Games.Beatmania.Pc.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Beatmania.Pc.Streamers;

[Service]
public class BeatmaniaPcAudioEntryStreamWriter : IBeatmaniaPcAudioEntryStreamWriter
{
    public void Write(Stream target, BeatmaniaPcAudioEntry entry)
    {
        var writer = new BinaryWriter(target);
        writer.Write(0x39584432); // 2DX9
        writer.Write(0x14 + entry.ExtraInfo.Length);
        writer.Write(entry.Data.Length);
        writer.Write((short) 0);
        writer.Write((short) entry.Channel);
        writer.Write((short) entry.Panning);
        writer.Write((short) entry.Volume);
        writer.Write(entry.ExtraInfo.Span);
        writer.Write(entry.Data.Span);
    }
}