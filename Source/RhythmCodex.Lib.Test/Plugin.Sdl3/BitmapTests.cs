using System.IO;
using NUnit.Framework;
using RhythmCodex.Plugin.Sdl3.Graphics;
using Shouldly;

namespace RhythmCodex.Plugin.Sdl3;

[TestFixture]
public class BitmapTests : BaseIntegrationFixture<BitmapStreamReader>
{
    [Test]
    public void test1()
    {
        var stream = new MemoryStream(GetEmbeddedResource("Png.example.png"));
        var bitmap = Subject.Read(stream);
        bitmap.Width.ShouldBe(172);
        bitmap.Height.ShouldBe(178);
    }
}