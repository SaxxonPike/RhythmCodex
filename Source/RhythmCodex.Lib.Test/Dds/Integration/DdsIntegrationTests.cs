using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Dds.Converters;
using RhythmCodex.Dds.Streamers;
using RhythmCodex.Gdi.Streamers;

namespace RhythmCodex.Dds.Integration;

[TestFixture]
public class DdsIntegrationTests : BaseIntegrationFixture
{
    [Test]
    [Explicit]
    [TestCase("uncompressed")]
    [TestCase("dxt1")]
    public void Test(string resource)
    {
        var data = GetArchiveResource($"Dds.{resource}.zip")
            .First()
            .Value;
        var mem = new MemoryStream(data);

        var reader = Resolve<IDdsStreamReader>();
        var decoder = Resolve<IDdsBitmapDecoder>();
        var writer = Resolve<IPngStreamWriter>();
            
        var inputImage = reader.Read(mem, (int) mem.Length);
        var decodedImage = decoder.Decode(inputImage);

        using var outStream =
            new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"{resource}.png"), FileMode.Create);
        writer.Write(outStream, decodedImage);
        outStream.Flush();
    }
}