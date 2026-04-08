using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Games.Beatmania.Ps2.Converters;
using RhythmCodex.Games.Beatmania.Ps2.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Vag.Models;
using RhythmCodex.Sounds.Vag.Streamers;
using RhythmCodex.Utils.Cursors;

namespace RhythmCodex.Games.Beatmania.Ps2.Streamers;

[Service]
public class BeatmaniaPs2NewKeysoundStreamReader(
    IVagStreamReader vagStreamReader,
    IBeatmaniaPs2FrequencyConverter frequencyConverter)
    : IBeatmaniaPs2NewKeysoundStreamReader
{
    private static List<BeatmaniaPs2NewInstrument> ReadInstrumentTable(
        Stream stream,
        BeatmaniaPs2NewInstrumentHeader instrumentHeader)
    {
        Span<byte> instrumentTableData = stackalloc byte[instrumentHeader.BlockCount * 0x800 - 0x10];
        stream.ReadExactly(instrumentTableData);

        var result = new List<BeatmaniaPs2NewInstrument>();
        var count = instrumentTableData.Length / 0x10;

        for (var i = 0; i < count; i++)
        {
            var item = instrumentTableData[(i * 0x10)..];
            var sampleChannelCount = item[3];

            if (sampleChannelCount == 0)
                continue;

            result.Add(new BeatmaniaPs2NewInstrument
            {
                Index = i,
                Flags00 = item[0],
                PlaybackChannel = item[1],
                Flags02 = item[2],
                SampleChannelCount = sampleChannelCount,
                Unknown04 = item[4..].AsS32L(),
                SampleChannel0Pan = item[8],
                SampleChannel1Pan = item[9],
                SampleNumber = item[10..].AsU16L(),
                Volume = item[12],
                Unknown0D = item[13],
                Unknown0E = item[14..].AsU16L()
            });
        }

        return result;
    }

    private static Dictionary<int, BeatmaniaPs2NewSample> ReadSampleTable(
        Stream stream,
        BeatmaniaPs2NewSampleHeader sampleHeader)
    {
        Span<byte> sampleTableData = stackalloc byte[sampleHeader.SampleCount * 0x10];
        stream.ReadExactly(sampleTableData);

        var result = new Dictionary<int, BeatmaniaPs2NewSample>();
        var count = sampleTableData.Length / 0x10;

        for (var i = 0; i < count; i++)
        {
            var item = sampleTableData[(i * 0x10)..];
            var channelCount = item[8];

            if (channelCount == 0)
                continue;

            result.Add(i, new BeatmaniaPs2NewSample
            {
                Index = i,
                SampleOffset = item.AsS32L(),
                SampleLength = item[4..].AsS32L(),
                ChannelCount = channelCount,
                Unknown09 = item[9],
                FineFreq = item[10],
                CoarseFreq = item[11],
                Unknown0C = item[12..].AsS32L()
            });
        }

        return result;
    }

    public BeatmaniaPs2KeysoundSet Read(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[0x10];

        //
        // Read the instrument table.
        //

        stream.ReadExactly(buffer);

        var instrumentHeader = new BeatmaniaPs2NewInstrumentHeader
        {
            Identifier = buffer.AsS32L(),
            BlockCount = buffer[4..].AsS32L(),
            Volume = buffer[8],
            Unknown09 = buffer[9],
            Unknown0A = buffer[10..].AsU16L(),
            Unknown0C = buffer[12..].AsS32L()
        };

        var instruments = ReadInstrumentTable(stream, instrumentHeader);

        //
        // Read the sample table.
        //

        stream.ReadExactly(buffer);

        var sampleHeader = new BeatmaniaPs2NewSampleHeader
        {
            SampleCount = buffer.AsS32L(),
            TotalSize = buffer[4..].AsS32L(),
            Unknown08 = buffer[8..].AsS32L(),
            Unknown0C = buffer[12..].AsS32L()
        };

        var samples = ReadSampleTable(stream, sampleHeader);

        //
        // Read the sample wave block. Decrypt if necessary.
        //

        stream.ReadExactly(buffer);

        using var sampleWaveDataHandle = stream.ReadIntoPool(sampleHeader.TotalSize);
        var sampleWaveDataMemory = sampleWaveDataHandle.Memory[..sampleHeader.TotalSize];
        var sampleWaveData = sampleWaveDataMemory.Span;

        if (buffer.AsS32L() != 0)
        {
            for (var i = 0; i < sampleWaveData.Length; i += 0x10)
            {
                unchecked
                {
                    sampleWaveData[i + 0] ^= buffer[0];
                    sampleWaveData[i + 1] ^= buffer[1];
                    sampleWaveData[i + 2] -= buffer[2];
                    sampleWaveData[i + 3] -= buffer[3];
                }
            }
        }

        //
        // Convert the tables and sample wave block to a keysound dictionary.
        //

        var sampleWaveStream = new ReadOnlyMemoryStream(sampleWaveDataMemory);

        var keysounds = instruments.Select(instrument =>
        {
            if (!samples.TryGetValue(instrument!.SampleNumber, out var sample))
            {
                return new BeatmaniaPs2Keysound
                {
                    Index = instrument.Index + 1,
                    SampleNumber = instrument.SampleNumber,
                    Channel = instrument.PlaybackChannel,
                    Volume = instrumentHeader.Volume * instrument.Volume,
                    VolumeScale = 10000,
                    PanningLeft = instrument.SampleChannel0Pan,
                    PanningRight = instrument.SampleChannel1Pan,
                    SampleType = instrument.Flags00,
                    Data = []
                };
            }

            var sampleWaves = new List<VagChunk>();
            Span<int> sampleOffsets = stackalloc int[sample.ChannelCount];

            if (sample.SampleLength > 0)
            {
                for (var i = 0; i < sample.ChannelCount; i++)
                {
                    sampleWaveStream.Position = sample.SampleOffset + i * sample.SampleLength;
                    sampleOffsets[i] = (int)sampleWaveStream.Position;
                    if (vagStreamReader.Read(sampleWaveStream, 1, 0) is { } vag)
                        sampleWaves.Add(vag);
                }
            }

            var calculatedFreq = (float)frequencyConverter.Convert(sample.CoarseFreq, sample.FineFreq);

            return new BeatmaniaPs2Keysound
            {
                Index = instrument.Index + 1,
                SampleNumber = instrument.SampleNumber,
                Channel = instrument.PlaybackChannel,
                Volume = instrumentHeader.Volume * instrument.Volume,
                VolumeScale = 10000,
                PanningLeft = instrument.SampleChannel0Pan,
                PanningRight = instrument.SampleChannel1Pan,
                SampleType = instrument.Flags00,
                FrequencyLeft = calculatedFreq,
                FrequencyRight = calculatedFreq,
                OffsetLeft = sampleOffsets.Length > 0 ? sampleOffsets[0] : 0,
                OffsetRight = sampleOffsets.Length > 1 ? sampleOffsets[1] : 0,
                Data = sampleWaves
            };
        }).ToList();

        return new BeatmaniaPs2KeysoundSet
        {
            Keysounds = keysounds
        };
    }
}