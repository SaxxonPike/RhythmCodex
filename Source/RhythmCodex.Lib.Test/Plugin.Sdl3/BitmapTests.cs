using System.IO;
using NUnit.Framework;
using RhythmCodex.Plugin.Sdl3.Graphics;
using Shouldly;

namespace RhythmCodex.Plugin.Sdl3;

[TestFixture]
public class BitmapStreamReaderTests : BaseIntegrationFixture<BitmapStreamReader>
{
    [Test]
    public void Read_ReadsPng()
    {
        var stream = new MemoryStream(GetEmbeddedResource("Png.example.png"));
        var bitmap = Subject.Read(stream);
        bitmap.Width.ShouldBe(172);
        bitmap.Height.ShouldBe(178);
    }
}