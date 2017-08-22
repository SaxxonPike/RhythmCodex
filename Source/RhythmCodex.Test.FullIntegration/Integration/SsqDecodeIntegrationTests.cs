using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Attributes;
using RhythmCodex.Charting;
using RhythmCodex.Extensions;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Streamers;

namespace RhythmCodex.Integration
{
    [TestFixture]
    public class SsqDecodeIntegrationTests : BaseIntegrationFixture
    {
        private IEnumerable<IChart> DecodeCharts(byte[] data)
        {
            var ssqDecoder = new SsqDecoder(
                new TimingChunkDecoder(),
                new TimingEventDecoder(),
                new StepChunkDecoder(),
                new StepEventDecoder(
                    new DdrStandardPanelMapper()),
                new TriggerChunkDecoder(),
                new TriggerEventDecoder());

            var ssqStreamer = new SsqStreamReader(
                new ChunkStreamReader());

            using (var mem = new MemoryStream(data))
            {
                return ssqDecoder.Decode(ssqStreamer.Read(mem)).ToArray();
            }
        }

        private string EncodeCharts(IList<IChart> charts)
        {
            var smEncoder = new SmEncoder(
                new NoteEncoder(), 
                new NoteCommandStringEncoder(), 
                new GrooveRadarEncoder());

            var commands = smEncoder.Encode(new Metadata(), charts).AsList();

            var smStreamer = new SmStreamWriter();

            using (var mem = new MemoryStream())
            {
                smStreamer.Write(mem, commands);
                mem.Flush();
                mem.Position = 0;
                var reader = new StreamReader(mem);
                return reader.ReadToEnd();
            }
        }

        [Test]
        public void Convert()
        {
            var data = GetArchiveResource("RhythmCodex.Data.freeze.zip")
                .First()
                .Value;
            var charts = DecodeCharts(data).AsList();
            var sm = EncodeCharts(charts);
            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "test.sm"), sm);
        }
    }
}
