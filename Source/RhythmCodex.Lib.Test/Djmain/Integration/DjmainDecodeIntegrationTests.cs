using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Shouldly;
using NUnit.Framework;
using RhythmCodex.Djmain.Converters;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.Djmain.Integration;

[TestFixture]
public class DjmainDecodeIntegrationTests : BaseIntegrationFixture
{
    private DjmainArchive DecodeChunk(byte[] data, DjmainChunkFormat format)
    {
        return Resolve<IDjmainDecoder>().Decode(new DjmainChunk
        {
            Data = data,
            Format = format
        }, new DjmainDecodeOptions
        {
            DisableAudio = false,
            DoNotConsolidateSamples = false
        });
    }

    [Test]
    [Explicit]
    public void Test1()
    {
        var data = GetArchiveResource("Djmain.popn1.zip")
            .First()
            .Value;

        var archive = DecodeChunk(data, DjmainChunkFormat.Popn1);
            
        var sounds = archive.Samples.ToDictionary(
            s => $"{(int)s[NumericData.SampleMap]:00}_{(int)s[NumericData.Id]:0000}.wav", 
            Resolve<IRiffPcm16SoundEncoder>().Encode);

        var outPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "UnitTest", "out");
        Directory.CreateDirectory(outPath);
            
        foreach (var kv in sounds)
        {
            using var stream =
                new FileStream(Path.Combine(outPath, kv.Key),
                    FileMode.Create);
            var sound = kv.Value;
            Resolve<IRiffStreamWriter>().Write(stream, sound);
        }
    }
        
    [Test]
    [Explicit]
    public void Test2()
    {
        var streamer = Resolve<IDjmainChunkStreamReader>();
        using var stream = File.OpenRead(@"Z:\User Data\Bemani\Beatmania Non-PC\bm1stmix.zip");
        using var zipStream = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = zipStream.Entries.Single();
        using var entryStream = entry.Open();
        var chunks = streamer.Read(entryStream);

        foreach (var chunk in chunks.AsParallel())
            this.WriteFile(chunk.Data, Path.Combine("djmain1st", $"{chunk.Id:D4}.djmain"));
    }
        
    [Test]
    public void Decode_DecodesBgm(
        [Values(0x05, 0x15, 0x25, 0x35, 0x45, 0x55, 0x65, 0x75, 0x85, 0x95, 0xA5, 0xB5, 0xC5, 0xD5, 0xE5, 0xF5)] byte param0, 
        [Values] DjmainChartType chartType)
    {
        // Arrange.
        var subject = Resolve<IDjmainChartDecoder>();
        var expectedPanning = Math.Max(0, (param0 >> 4) - 1);
        var offset = Create<ushort>();
        var param1 = Create<byte>();
        var data = new[]
        {
            new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            }
        };

        // Act.
        var output = subject.Decode(data, Create<DjmainChartType>());

        // Assert.
        output.Events.Count.ShouldBe(1);
        var ev = output.Events.Single();
        ev[NumericData.PlaySound].Value.ShouldBe(param1 - 1);
        ev[NumericData.Panning].Value.ShouldBe(new BigRational(expectedPanning, 14));
    }

    [Test]
    public void Decode_DecodesEnd()
    {
        // Arrange.
        var subject = Resolve<IDjmainChartDecoder>();
        var offset = Create<ushort>();
        const int param0 = 0x04;
        var param1 = Create<byte>();
        var data = new[]
        {
            new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            }
        };

        // Act.
        var output = subject.Decode(data, Create<DjmainChartType>());

        // Assert.
        output.Events.Count.ShouldBe(1);
        var ev = output.Events.Single();
        ev[FlagData.End]!.Value.ShouldBeTrue();
    }

    [Test]
    [TestCase(0xE0, 0)]
    [TestCase(0xF0, 1)]
    public void Decode_DecodesFreeScratch_WhenChartTypeIsBeatmania(byte param0, int expectedPlayer)
    {
        // Arrange.
        var subject = Resolve<IDjmainChartDecoder>();
        var offset = Create<ushort>();
        var param1 = Create<byte>();
        var data = new[]
        {
            new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            }
        };

        // Act.
        var output = subject.Decode(data, DjmainChartType.Beatmania);

        // Assert.
        output.Events.Count.ShouldBe(1);
        var ev = output.Events.Single();
        ev[FlagData.FreeZone].ShouldBe(true);
        ev[NumericData.Player].Value.ShouldBe(expectedPlayer);
    }

    [Test]
    [TestCase(0xC0, 0)]
    [TestCase(0xD0, 1)]
    public void Decode_DecodesMeasure_WhenChartTypeIsBeatmania(byte param0, int expectedPlayer)
    {
        // Arrange.
        var subject = Resolve<IDjmainChartDecoder>();
        var offset = Create<ushort>();
        var param1 = Create<byte>();
        var data = new[]
        {
            new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            }
        };

        // Act.
        var output = subject.Decode(data, DjmainChartType.Beatmania);

        // Assert.
        output.Events.Count.ShouldBe(1);
        var ev = output.Events.Single();
        ev[FlagData.Measure].ShouldBe(true);
        ev[NumericData.Player].Value.ShouldBe(expectedPlayer);
    }

    [Test]
    [TestCase(0x00, 0, 0)]
    [TestCase(0x10, 1, 0)]
    [TestCase(0x20, 0, 1)]
    [TestCase(0x30, 1, 1)]
    [TestCase(0x40, 0, 2)]
    [TestCase(0x50, 1, 2)]
    [TestCase(0x60, 0, 3)]
    [TestCase(0x70, 1, 3)]
    [TestCase(0x80, 0, 4)]
    [TestCase(0x90, 1, 4)]
    public void Decode_DecodesNote_WhenChartTypeIsBeatmania(byte param0, int expectedPlayer, int expectedColumn)
    {
        // Arrange.
        var subject = Resolve<IDjmainChartDecoder>();
        var offset = Create<ushort>();
        var param1 = Create<byte>();
        var data = new[]
        {
            new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            }
        };

        // Act.
        var output = subject.Decode(data, DjmainChartType.Beatmania);

        // Assert.
        output.Events.Count.ShouldBe(1);
        var ev = output.Events.Single();
        ev[FlagData.Note].ShouldBe(true);
        ev[NumericData.Player].Value.ShouldBe(expectedPlayer);
        ev[NumericData.Column].Value.ShouldBe(expectedColumn);
    }

    [Test]
    [TestCase(0x01, 0, 0)]
    [TestCase(0x11, 1, 0)]
    [TestCase(0x21, 0, 1)]
    [TestCase(0x31, 1, 1)]
    [TestCase(0x41, 0, 2)]
    [TestCase(0x51, 1, 2)]
    [TestCase(0x61, 0, 3)]
    [TestCase(0x71, 1, 3)]
    [TestCase(0x81, 0, 4)]
    [TestCase(0x91, 1, 4)]
    public void Decode_DecodesNoteSoundSelect_WhenChartTypeIsBeatmania(byte param0, int expectedPlayer, int expectedColumn)
    {
        // Arrange.
        var subject = Resolve<IDjmainChartDecoder>();
        var offset = Create<ushort>();
        var param1 = Create<byte>();
        var data = new[]
        {
            new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            }
        };

        // Act.
        var output = subject.Decode(data, DjmainChartType.Beatmania);

        // Assert.
        output.Events.Count.ShouldBe(1);
        var ev = output.Events.Single();
        ev[NumericData.LoadSound].Value.ShouldBe(param1 - 1);
        ev[NumericData.Player].Value.ShouldBe(expectedPlayer);
        ev[NumericData.Column].Value.ShouldBe(expectedColumn);
    }

    [Test]
    [TestCase(0xA0, 0)]
    [TestCase(0xB0, 1)]
    public void Decode_DecodesScratch_WhenChartTypeIsBeatmania(byte param0, int expectedPlayer)
    {
        // Arrange.
        var subject = Resolve<IDjmainChartDecoder>();
        var offset = Create<ushort>();
        var param1 = Create<byte>();
        var data = new[]
        {
            new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            }
        };

        // Act.
        var output = subject.Decode(data, DjmainChartType.Beatmania);

        // Assert.
        output.Events.Count.ShouldBe(1);
        var ev = output.Events.Single();
        ev[FlagData.Scratch].ShouldBe(true);
        ev[NumericData.Player].Value.ShouldBe(expectedPlayer);
    }

    [Test]
    [TestCase(0xA1, 0)]
    [TestCase(0xB1, 1)]
    [TestCase(0xE1, 0)]
    [TestCase(0xF1, 1)]
    public void Decode_DecodesScratchSoundSelect_WhenChartTypeIsBeatmania(byte param0, int expectedPlayer)
    {
        // Arrange.
        var subject = Resolve<IDjmainChartDecoder>();
        var offset = Create<ushort>();
        var param1 = Create<byte>();
        var data = new[]
        {
            new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            }
        };

        // Act.
        var output = subject.Decode(data, DjmainChartType.Beatmania);

        // Assert.
        output.Events.Count.ShouldBe(1);
        var ev = output.Events.Single();
        ev[FlagData.Scratch].Value.ShouldBeTrue();
        ev[NumericData.LoadSound].Value.ShouldBe(param1 - 1);
        ev[NumericData.Player].Value.ShouldBe(expectedPlayer);
    }

    [Test]
    [TestCase(0x02, 0x01, 0x001)]
    [TestCase(0x02, 0x04, 0x004)]
    [TestCase(0x02, 0x11, 0x011)]
    [TestCase(0x02, 0x44, 0x044)]
    [TestCase(0x12, 0x01, 0x101)]
    [TestCase(0x12, 0x04, 0x104)]
    [TestCase(0x12, 0x11, 0x111)]
    [TestCase(0x12, 0x44, 0x144)]
    [TestCase(0x42, 0x01, 0x401)]
    [TestCase(0x42, 0x04, 0x404)]
    [TestCase(0x42, 0x11, 0x411)]
    [TestCase(0x42, 0x44, 0x444)]
    public void Decode_DecodesTempo(byte param0, byte param1, int expectedTempo)
    {
        // Arrange.
        var subject = Resolve<IDjmainChartDecoder>();
        var offset = Create<ushort>();
        var data = new[]
        {
            new DjmainChartEvent
            {
                Offset = offset,
                Param0 = param0,
                Param1 = param1
            }
        };

        // Act.
        var output = subject.Decode(data, Create<DjmainChartType>());

        // Assert.
        output.Events.Count.ShouldBe(1);
        var ev = output.Events.Single();
        ev[NumericData.Bpm].Value.ShouldBe(expectedTempo);
    }

}