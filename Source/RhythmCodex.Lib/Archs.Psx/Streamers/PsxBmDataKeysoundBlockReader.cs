using System;
using System.IO;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Psx.Streamers;

/// <inheritdoc />
[Service]
public class PsxBmDataKeysoundBlockReader : IPsxBmDataKeysoundBlockReader
{
    /// <inheritdoc />
    public PsxBmDataKeysoundBlock Read(Stream stream)
    {
        //
        // There are two patches in a block. The first describes the keysounds,
        // and the second contains the audio.
        //

        return new PsxBmDataKeysoundBlock
        {
            Patches =
            [
                ReadPatch(stream),
                ReadPatch(stream)
            ]
        };
    }

    /// <summary>
    /// Reads one patch from the block.
    /// </summary>
    private static PsxBmDataKeysoundBlockPatch ReadPatch(Stream stream)
    {
        Span<byte> patchHeader = stackalloc byte[0x10];

        stream.ReadExactly(patchHeader);

        //
        // These values are big endian, unlike all the rest.
        //

        var address = ReadInt32BigEndian(patchHeader);
        var length = ReadInt32BigEndian(patchHeader[4..]);
        var data = new byte[length];
        
        stream.ReadExactly(data);

        return new PsxBmDataKeysoundBlockPatch
        {
            Address = address,
            Length = length,
            Data = data
        };
    }
}