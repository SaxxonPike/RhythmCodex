using System.IO;
using NUnit.Framework;
using RhythmCodex.Games.Beatmania.Pc.Models;
using RhythmCodex.Games.Beatmania.Pc.Streamers;
using RhythmCodex.Infrastructure;
using Shouldly;

namespace RhythmCodex.Games.Beatmania.Streamers;

[TestFixture]
public class BeatmaniaPcAudioEntryStreamReaderTests : BaseUnitTestFixture<BeatmaniaPcAudioEntryStreamReader,
    IBeatmaniaPcAudioEntryStreamReader>
{
    private static Stream CreateData(BeatmaniaPcAudioEntry entry)
    {
        var mem = new MemoryStream();
        var writer = new BinaryWriter(mem);

        writer.Write(0x39584432); // 2DX9
        writer.Write(0x14 + entry.ExtraInfo.Length); // header length
        writer.Write(entry.Data.Length);
        writer.Write((short) entry.Reserved);
        writer.Write((short) entry.Channel);
        writer.Write((short) entry.Panning);
        writer.Write((short) entry.Volume);
        writer.Write(entry.ExtraInfo.Span);
        writer.Write(entry.Data.Span);

        mem.Position = 0;
        return mem;
    }

    [Test]
    public void Read_ShouldConvertReserved()
    {
        // Arrange.
        var entry = Create<BeatmaniaPcAudioEntry>();

        // Act.
        var observed = Subject.Read(CreateData(entry));

        // Assert.
        observed.Reserved.ShouldBe(entry.Reserved);
    }

    [Test]
    public void Read_ShouldConvertChannel()
    {
        // Arrange.
        var entry = Create<BeatmaniaPcAudioEntry>();

        // Act.
        var observed = Subject.Read(CreateData(entry));

        // Assert.
        observed.Channel.ShouldBe(entry.Channel);
    }

    [Test]
    public void Read_ShouldConvertPanning()
    {
        // Arrange.
        var entry = Create<BeatmaniaPcAudioEntry>();

        // Act.
        var observed = Subject.Read(CreateData(entry));

        // Assert.
        observed.Panning.ShouldBe(entry.Panning);
    }

    [Test]
    public void Read_ShouldConvertVolume()
    {
        // Arrange.
        var entry = Create<BeatmaniaPcAudioEntry>();

        // Act.
        var observed = Subject.Read(CreateData(entry));

        // Assert.
        observed.Volume.ShouldBe(entry.Volume);
    }

    [Test]
    public void Read_ShouldConvertExtraInfo()
    {
        // Arrange.
        var entry = Create<BeatmaniaPcAudioEntry>();

        // Act.
        var observed = Subject.Read(CreateData(entry));

        // Assert.
        observed.ExtraInfo.ToArray().ShouldBeEquivalentTo(entry.ExtraInfo.ToArray());
    }

    [Test]
    public void Read_ShouldConvertData()
    {
        // Arrange.
        var entry = Create<BeatmaniaPcAudioEntry>();

        // Act.
        var observed = Subject.Read(CreateData(entry));

        // Assert.
        observed.Data.ToArray().ShouldBeEquivalentTo(entry.Data.ToArray());
    }

    [Test]
    public void Read_ShouldThrow_WhenIdIsInvalid()
    {
        // Arrange.
        var entry = Create<BeatmaniaPcAudioEntry>();
        var data = CreateData(entry);
        data.WriteByte(0);
        data.Position = 0;

        // Act.
        Should.Throw<RhythmCodexException>(() => Subject.Read(data));
    }
}