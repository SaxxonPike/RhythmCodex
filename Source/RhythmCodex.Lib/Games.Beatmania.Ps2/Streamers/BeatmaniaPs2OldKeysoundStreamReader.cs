using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Streamers;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

/// <inheritdoc />
[Service]
public class BeatmaniaPs2OldKeysoundStreamReader(IVagStreamReader vagStreamReader)
    : IBeatmaniaPs2OldKeysoundStreamReader
{
    /// <summary>
    /// Byte offset adjustment for sample type 2.
    /// </summary>
    private const int Type2Base = -0xF030;

    /// <summary>
    /// Byte offset adjustment for sample type 3.
    /// </summary>
    private const int Type3Base = 0x3FF0;

    /// <summary>
    /// Byte offset adjustment for sample type 4.
    /// </summary>
    private const int Type4Base = -0xF030;

    /// <inheritdoc />
    public BeatmaniaPs2KeysoundSet Read(Stream stream)
    {
        //
        // Read the header data (32 bytes.)
        //

        var reader = new BinaryReader(stream);
        Span<int> header = stackalloc int[8];

        for (var i = 0; i < 8; i++)
            header[i] = reader.ReadInt32();

        var length = header[0];
        var type3Offset = header[1];

        //
        // Read the sample hunk.
        //

        using var hunkMem = stream.ReadIntoTempStream(length - 0x20);

        //
        // Process the sample table. There are up to 511 entries possible.
        //

        var entries = new List<BeatmaniaPs2Keysound>();

        for (var i = 0; i < 511; i++)
        {
            var item = hunkMem.Span[(i * 0x20)..];
            var sampleType = item[7];

            //
            // Short circuit if the sample type is not known.
            //

            if (sampleType == 0)
                continue;

            var result = new BeatmaniaPs2Keysound
            {
                SampleNumber = item.AsS16L(),
                Reserved0 = item[2..].AsS16L(),
                Channel = item[4],
                Volume = item[5],
                PanningLeft = item[6],
                SampleType = sampleType,
                FrequencyLeft = item[8..].AsS32L(),
                FrequencyRight = item[12..].AsS32L(),
                OffsetLeft = item[16..].AsS32L(),
                OffsetRight = item[20..].AsS32L(),
                Reserved1 = item[24..].AsS32L(),
                Reserved2 = item[28..].AsS32L(),

                Index = item.AsS16L(),
                PanningRight = item[6]
            };

            //
            // Read sample data.
            //

            switch (result.SampleType)
            {
                case 2:
                {
                    hunkMem.Position = result.OffsetLeft + Type2Base;
                    result.Data = vagStreamReader.Read(hunkMem, 1, 0) is { } chunk
                        ? [chunk]
                        : [];
                    break;
                }
                case 3:
                {
                    hunkMem.Position = type3Offset + result.OffsetLeft + Type3Base;
                    result.Data = vagStreamReader.Read(hunkMem, 1, 0) is { } chunk
                        ? [chunk]
                        : [];
                    break;
                }
                case 4:
                {
                    hunkMem.Position = result.OffsetLeft + Type4Base;
                    var dataLeft = vagStreamReader.Read(hunkMem, 1, 0);
                    hunkMem.Position = result.OffsetRight + Type4Base;
                    var dataRight = vagStreamReader.Read(hunkMem, 1, 0);
                    result.Data = dataLeft is not null && dataRight is not null
                        ? [dataLeft, dataRight]
                        : [];
                    break;
                }
                default:
                {
                    continue;
                }
            }

            entries.Add(result);
        }

        return new BeatmaniaPs2KeysoundSet
        {
            Keysounds = entries
        };
    }
}