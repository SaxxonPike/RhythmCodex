using System;
using System.IO;
using RhythmCodex.Games.Mgs.Models;
using RhythmCodex.IoC;

namespace RhythmCodex.Games.Mgs.Streamers;

/// <inheritdoc />
[Service]
public sealed class MgsSdSoundBankBlockReader : IMgsSdSoundBankBlockReader
{
    /// <inheritdoc />
    public MgsSdSoundBankBlock Read(Stream stream)
    {
        //
        // There are two patches in a block. The first describes the keysounds,
        // and the second contains the audio.
        //

        return new MgsSdSoundBankBlock
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
    private static MgsSdSoundBankBlockPatch ReadPatch(Stream stream)
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

        return new MgsSdSoundBankBlockPatch
        {
            Address = address,
            Length = length,
            Data = data
        };
    }
}