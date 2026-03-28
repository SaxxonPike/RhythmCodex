using System;
using System.IO;
using RhythmCodex.Archs.Psx.Model;
using RhythmCodex.IoC;

namespace RhythmCodex.Archs.Psx.Streamers;

/// <inheritdoc />
[Service]
public sealed class PsxMgsSoundBankBlockReader : IPsxMgsSoundBankBlockReader
{
    /// <inheritdoc />
    public PsxMgsSoundBankBlock Read(Stream stream)
    {
        //
        // There are two patches in a block. The first describes the keysounds,
        // and the second contains the audio.
        //

        return new PsxMgsSoundBankBlock
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
    private static PsxMgsSoundBankBlockPatch ReadPatch(Stream stream)
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

        return new PsxMgsSoundBankBlockPatch
        {
            Address = address,
            Length = length,
            Data = data
        };
    }
}