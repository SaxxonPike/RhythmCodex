using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.BeatmaniaPsx.Streamers;
using RhythmCodex.Bms.Converters;

namespace RhythmCodex.BeatmaniaPsx.Integration
{
    [TestFixture]
    public class BeatmaniaPsxBmDataStreamReaderIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
        [TestCase(@"Z:\BMDATA.PAK")]
        public void Test1(string fileName)
        {
            var reader = Resolve<IBeatmaniaPsxBmDataStreamReader>();
            var encoder = Resolve<IBmsEncoder>();
            
            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var output = reader.Read(file, (int) file.Length);

                var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "bmdata");
                var folderIndex = 0;
                foreach (var folder in output)
                {
                    var fileIndex = 0;
                    Directory.CreateDirectory(Path.Combine(outputFolder, $"{folderIndex:X4}"));
                    foreach (var folderFile in folder.Files)
                    {
                        var data = folderFile.Data;
                        var extension = "bin";

                        if (data.Length >= 4 && data.Skip(data.Length - 4)
                                .SequenceEqual(new byte[] {0xFF, 0x7F, 0x00, 0x00}))
                            extension = "cs5";
                        
                        File.WriteAllBytes(Path.Combine(outputFolder, $"{folderIndex:X4}", $"{fileIndex:X4}.{extension}"), folderFile.Data);
                        fileIndex++;
                    }

                    folderIndex++;
                }
            }
        }
    }
}