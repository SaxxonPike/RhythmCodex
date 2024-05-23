using System;
using System.IO;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers;
// todo: more event types
// url: https://github.com/MonoGame/MonoGame/blob/master/MonoGame.Framework/Audio/Xact/XactClip.cs
// license: ms-pl

[Service]
public class XsbSoundClipStreamReader : IXsbSoundClipStreamReader
{
    public XsbSoundClip Read(Stream stream)
    {
        var reader = new BinaryReader(stream);

        var result = new XsbSoundClip
        {
            Volume = reader.ReadByte(),
            ClipOffset = reader.ReadInt32()
        };

        var oldPosition = stream.Position;

        if (stream.Position + 4 < result.ClipOffset)
        {
            // these are always read in MonoGame but my XSB examples lack them entirely
            result.FilterFlags = reader.ReadInt16();
            result.FilterFrequency = reader.ReadInt16();
        }

        stream.Position = result.ClipOffset;

        var numEvents = reader.ReadByte();
        var events = new XsbSoundClipEvent[numEvents];

        for (var i = 0; i < numEvents; i++)
        {
            var ev = new XsbSoundClipEvent
            {
                Info = reader.ReadInt32(),
                RandomOffset = reader.ReadInt16()
            };

            switch (ev.EventId)
            {
                case 1:
                {
                    ev.Flags0 = reader.ReadByte();
                    ev.Flags1 = reader.ReadByte();
                    ev.TrackIndex = reader.ReadInt16();
                    ev.WaveBankIndex = reader.ReadByte();
                    ev.LoopCount = reader.ReadByte();
                    ev.PanAngle = reader.ReadInt16();
                    ev.PanArc = reader.ReadInt16();
                    break;
                }
                default:
                {
                    throw new NotImplementedException($"Event Id 0x{ev.EventId:X2} is not supported.");
                }
            }

            events[i] = ev;
        }

        stream.Position = oldPosition;
        result.Events = events;
        return result;
    }
}