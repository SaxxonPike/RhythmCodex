using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Models;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Beatmania.Streamers;

[Service]
public class BeatmaniaPs2NewKeysoundStreamReader(IVagStreamReader vagStreamReader)
    : IBeatmaniaPs2NewKeysoundStreamReader
{
    private readonly IVagStreamReader _vagStreamReader = vagStreamReader;

    public BeatmaniaPs2KeysoundSet Read(Stream stream)
    {
        var reader = new BinaryReader(stream);

        var instrumentHeader = new BeatmaniaPs2NewInstrumentHeader
        {
            Identifier = reader.ReadInt32(),
            BlockCount = reader.ReadInt32(),
            VolumeLeft = reader.ReadByte(),
            VolumeRight = reader.ReadByte(),
            Unknown0A = reader.ReadUInt16(),
            Unknown0C = reader.ReadInt32()
        };

        var instrumentTableData = reader.ReadBytes(instrumentHeader.BlockCount * 0x800 - 0x10);

        using var instrumentTableReader = new BinaryReader(new MemoryStream(instrumentTableData));

        var instruments = Enumerable.Range(0, instrumentTableData.Length / 0x10).Select(i =>
        {
            var result = new BeatmaniaPs2NewInstrument
            {
                Index = i,
                Flags00 = instrumentTableReader.ReadByte(),
                PlaybackChannel = instrumentTableReader.ReadByte(),
                Flags02 = instrumentTableReader.ReadByte(),
                SampleChannelCount = instrumentTableReader.ReadByte(),
                Unknown04 = instrumentTableReader.ReadInt32(),
                VolumeLeft = instrumentTableReader.ReadByte(),
                VolumeRight = instrumentTableReader.ReadByte(),
                SampleNumber = instrumentTableReader.ReadUInt16(),
                Unknown0C = instrumentTableReader.ReadInt32()
            };

            return result.SampleChannelCount == 0 ? null : result;
        }).Where(entry => entry != null).ToList();

        var sampleHeader = new BeatmaniaPs2NewSampleHeader
        {
            SampleCount = reader.ReadInt32(),
            TotalSize = reader.ReadInt32(),
            Unknown08 = reader.ReadInt32(),
            Unknown0C = reader.ReadInt32()
        };

        var sampleTableData = reader.ReadBytes(sampleHeader.SampleCount * 0x10);
        var decryptKey = reader.ReadBytes(0x10);
        var sampleWaveData = reader.ReadBytes(sampleHeader.TotalSize);

        if (decryptKey[0] != 0 || decryptKey[1] != 0 || decryptKey[2] != 0 || decryptKey[3] != 0)
        {
            for (var i = 0; i < sampleWaveData.Length; i += 0x10)
            {
                unchecked
                {
                    sampleWaveData[i + 0] ^= decryptKey[i + 0];
                    sampleWaveData[i + 1] ^= decryptKey[i + 1];
                    sampleWaveData[i + 2] -= decryptKey[i + 2];
                    sampleWaveData[i + 3] -= decryptKey[i + 3];
                }
            }
        }

        var sampleTableReader = new BinaryReader(new MemoryStream(sampleTableData));
        var sampleWaveStream = new MemoryStream(sampleWaveData);

        var samples = Enumerable.Range(0, sampleHeader.SampleCount).Select(i =>
        {
            var result = new BeatmaniaPs2NewSample
            {
                Index = i,
                SampleOffset = sampleTableReader.ReadInt32(),
                SampleLength = sampleTableReader.ReadInt32(),
                ChannelCount = sampleTableReader.ReadByte(),
                Unknown09 = sampleTableReader.ReadByte(),
                FineFreq = sampleTableReader.ReadByte(),
                CoarseFreq = sampleTableReader.ReadByte(),
                Unknown0C = sampleTableReader.ReadInt32()
            };

            return result.ChannelCount == 0 ? null : result;
        }).Where(entry => entry != null).ToDictionary(x => x!.Index, x => x!);

        var keysounds = instruments.Select(instrument =>
        {
            if (!samples.TryGetValue(instrument!.SampleNumber, out var sample))
            {
                return new BeatmaniaPs2Keysound
                {
                    Index = instrument.Index,
                    SampleNumber = instrument.SampleNumber,
                    Channel = instrument.PlaybackChannel,
                    Volume = instrumentHeader.VolumeLeft,
                    Panning = 0x40,
                    SampleType = instrument.Flags00,
                    Data = []
                };
            }

            var sampleWaves = new List<VagChunk>();

            if (sample.SampleLength > 0)
            {
                for (var i = 0; i < sample.ChannelCount; i++)
                {
                    sampleWaveStream.Position = sample.SampleOffset + i * sample.SampleLength;
                    if (vagStreamReader.Read(sampleWaveStream, 1, 0) is { } vag)
                        sampleWaves.Add(vag);
                }
            }

            //
            // Source: "trust me bro."
            // It took forever to discover this is treated as a two-part frequency calculation:
            // a coarse-tuning (unsure the range) and fine-tuning (00-7F). An octave is represented
            // as 12 coarse steps (like the chromatic scale on a piano). The fine-tuning is then an
            // adjustment on top of that, which has a range of about 3 coarse steps over 127 values.
            // The final constant was brute forced and comes out to a value close to 1/255*6 if I
            // adjust to {coarse=0x3E, fine=0x44} -> ~44100. The old lookup table was empirically built:
            // compare exact frequencies from the old format to the new format for songs that were revived.
            // It turned out to be acceptable, but not accurate.
            //

            var freq = 1536000d / Math.Pow(2, sample.CoarseFreq / 12d - sample.FineFreq / 1531.155d);
            var roundedFreq = (int)Math.Round(freq);

            return new BeatmaniaPs2Keysound
            {
                Index = instrument.Index,
                SampleNumber = instrument.SampleNumber,
                Channel = instrument.PlaybackChannel,
                Volume = instrumentHeader.VolumeLeft,
                Panning = 0x40,
                SampleType = instrument.Flags00,
                FrequencyLeft = roundedFreq,
                FrequencyRight = roundedFreq,
                OffsetLeft = 0,
                OffsetRight = 0,
                PseudoLeft = 0,
                PseudoRight = 0,
                Data = sampleWaves
            };
        }).ToList();

        return new BeatmaniaPs2KeysoundSet
        {
            Keysounds = keysounds
        };
    }
}