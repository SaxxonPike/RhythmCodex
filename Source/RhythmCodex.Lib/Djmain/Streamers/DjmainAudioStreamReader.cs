﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.IoC;

namespace RhythmCodex.Djmain.Streamers;

[Service]
public class DjmainAudioStreamReader : IDjmainAudioStreamReader
{
    public Memory<byte> ReadDpcm(Stream stream)
    {
        return ReadDpcmStream(stream).ToArray();
    }

    public Memory<byte> ReadPcm16(Stream stream)
    {
        return DecodePcm16Stream(stream).ToArray();
    }

    public Memory<byte> ReadPcm8(Stream stream)
    {
        return DecodePcm8Stream(stream).ToArray();
    }

    private static IEnumerable<byte> ReadDpcmStream(Stream stream)
    {
        const int marker = DjmainConstants.DpcmEndMarker;
        var buffer = marker;

        void Fetch()
        {
            buffer >>= 8;
            var newByte = stream.ReadByte();
            if (newByte < 0)
                newByte = 0x88;
            buffer = (buffer & 0xFFFFFF) | (newByte << 24);
        }

        for (var i = 0; i < 4; i++)
            Fetch();

        while (buffer != marker)
        {
            yield return unchecked((byte) buffer);
            Fetch();
        }
    }

    private static IEnumerable<byte> DecodePcm16Stream(Stream stream)
    {
        const long marker = DjmainConstants.Pcm16EndMarker;
        var buffer0 = marker;
        var buffer1 = marker;

        void Fetch()
        {
            buffer0 = ((buffer0 >> 16) & 0xFFFFFFFFFFFF) | (buffer1 << 48);
            buffer1 = (buffer1 >> 16) & 0xFFFFFFFFFFFF;

            var newByte = stream.ReadByte();
            if (newByte == -1)
                newByte = 0x00;
            buffer1 |= (long) newByte << 48;

            newByte = stream.ReadByte();
            if (newByte == -1)
                newByte = 0x80;
            buffer1 |= (long) newByte << 56;
        }

        // Preload buffer.
        for (var i = 0; i < 8; i++)
            Fetch();

        // Load sample.
        while (buffer0 != marker || buffer1 != marker)
        {
            yield return unchecked((byte) buffer0);
            yield return unchecked((byte) (buffer0 >> 8));
            Fetch();
        }
    }

    private static IEnumerable<byte> DecodePcm8Stream(Stream stream)
    {
        const long marker = DjmainConstants.Pcm8EndMarker;
        var buffer = marker;

        void Fetch()
        {
            long newByte = stream.ReadByte();
            if (newByte < 0)
                newByte = 0x80;
            buffer = ((buffer >> 8) & 0xFFFFFFFFFFFFFF) | (newByte << 56);
        }

        // Preload buffer.
        for (var i = 0; i < 8; i++)
            Fetch();

        // Load sample.
        while (buffer != marker)
        {
            yield return unchecked((byte) buffer);
            Fetch();
        }
    }
}