using System;
using System.IO;
using RhythmCodex.Streamers;

namespace RhythmCodex.Beatmania.Streams;

public class BeatmaniaPs2NewAudioDecryptStream : PassthroughStream
{
    private readonly ReadOnlyMemory<byte> _key;

    public BeatmaniaPs2NewAudioDecryptStream(Stream baseStream, ReadOnlyMemory<byte> key) : base(baseStream)
    {
        _key = key;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var location = BaseStream.Position;
        var bytesRead = base.Read(buffer, offset, count);
        for (var i = 0; i < bytesRead; i++)
        {
            var lineLocation = unchecked((int)(location & 0xF));
            var bufferOffset = i + offset;
            switch (lineLocation)
            {
                case 0x0:
                case 0x1:
                    buffer[bufferOffset] ^= _key.Span[lineLocation];
                    break;
                case 0x2:
                case 0x3:
                    buffer[bufferOffset] = unchecked((byte)(buffer[bufferOffset] - _key.Span[lineLocation]));
                    break;
            }
        }

        return bytesRead;
    }
}