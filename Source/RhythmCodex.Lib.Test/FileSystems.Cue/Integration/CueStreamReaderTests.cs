using System.Collections.Generic;
using NUnit.Framework;
using RhythmCodex.FileSystems.Cue.Model;
using RhythmCodex.FileSystems.Cue.Streamers;
using Shouldly;

namespace RhythmCodex.FileSystems.Cue.Integration;

[TestFixture]
public class CueStreamReaderTests : BaseIntegrationFixture<CueStreamReader>
{
    [Test]
    public void TestReadCue()
    {
        var cue = new List<string>
        {
            "FILE \"testfile.bin\" BINARY",
            "TRACK 01 MODE2/2352",
            "PREGAP 00:02:00",
            "POSTGAP 00:03:00",
            "INDEX 01 00:02:00"
        };

        var observed = Subject.ReadCue(cue.ToStream());

        observed.ShouldBeEquivalentTo(new CueFile
        {
            Tracks =
            [
                new CueTrack
                {
                    Number = 1,
                    Indices =
                    {
                        { 1, 2 * 75 }
                    },
                    FileType = "BINARY",
                    FileName = "testfile.bin",
                    Type = "MODE2/2352",
                    Pregap = 2 * 75,
                    Postgap = 3 * 75,
                    ExtraLines =
                    [
                    ],
                    StoredBytesPerSector = 2352
                }
            ],
            ExtraLines = []
        });
    }
}