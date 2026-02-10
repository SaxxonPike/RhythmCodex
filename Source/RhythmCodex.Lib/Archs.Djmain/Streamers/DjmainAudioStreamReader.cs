using System;
using System.IO;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Djmain.Streamers;

[Service]
public class DjmainAudioStreamReader : IDjmainAudioStreamReader
{
    public Memory<byte> ReadDpcm(Stream stream)
    {
        const int marker = DjmainConstants.DpcmEndMarker;
        var buffer = marker;
        var mem = new MemoryStream();

        for (var i = 0; i < 4; i++)
            Fetch();

        while (buffer != marker)
        {
            mem.WriteByte(unchecked((byte)buffer));
            Fetch();
        }

        return mem.GetBuffer().AsMemory(0, (int)mem.Length);

        void Fetch()
        {
            buffer >>= 8;
            var newByte = stream.ReadByte();
            if (newByte < 0)
                newByte = 0x88;
            buffer = (buffer & 0xFFFFFF) | (newByte << 24);
        }
    }

    public Memory<byte> ReadPcm16(Stream stream)
    {
        const long marker = DjmainConstants.Pcm16EndMarker;
        var buffer0 = marker;
        var buffer1 = marker;
        var mem = new MemoryStream();

        // Preload buffer.
        for (var i = 0; i < 8; i++)
            Fetch();

        // Load sample.
        while (buffer0 != marker || buffer1 != marker)
        {
            mem.WriteByte(unchecked((byte)buffer0));
            mem.WriteByte(unchecked((byte)(buffer0 >> 8)));
            Fetch();
        }

        return mem.GetBuffer().AsMemory(0, (int)mem.Length);

        void Fetch()
        {
            Span<byte> inBuffer = stackalloc byte[2];
            buffer0 = ((buffer0 >> 16) & 0xFFFFFFFFFFFF) | (buffer1 << 48);
            buffer1 = (buffer1 >> 16) & 0xFFFFFFFFFFFF;

            if (stream.ReadAtLeast(inBuffer, 2, false) < 2)
            {
                inBuffer[0] = 0x00;
                inBuffer[1] = 0x80;
            }

            buffer1 |= (long)ReadUInt16LittleEndian(inBuffer) << 48;
        }
    }

    public Memory<byte> ReadPcm8(Stream stream)
    {
        const long marker = DjmainConstants.Pcm8EndMarker;
        var buffer = marker;
        var mem = new MemoryStream();

        // Preload buffer.
        for (var i = 0; i < 8; i++)
            Fetch();

        // Load sample.
        while (buffer != marker)
        {
            mem.WriteByte(unchecked((byte)buffer));
            Fetch();
        }

        return mem.GetBuffer().AsMemory(0, (int)mem.Length);

        void Fetch()
        {
            long newByte = stream.ReadByte();
            if (newByte < 0)
                newByte = 0x80;
            buffer = ((buffer >> 8) & 0xFFFFFFFFFFFFFF) | (newByte << 56);
        }
    }
}