using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Stepmania;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Model;
using RhythmCodex.Stepmania.Streamers;
using RhythmCodex.Vag.Converters;
using RhythmCodex.Vag.Heuristics;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Ddr.Integration
{
    public class DdrPs2FileDataIntegrationTests : BaseIntegrationFixture
    {
        [Test]
        [Explicit]
        public void test1()
        {
            using var source = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "us.filedata.bin"), FileMode.Open);
            var streamer = Resolve<IDdrPs2FileDataStepStreamReader>();
            var output = streamer.Read(source, source.Length);

            var tableDecoder = Resolve<IDdrPs2FileDataTableDecoder>();
            var table = tableDecoder.Decode(output);

            var ssqReader = Resolve<ISsqStreamReader>();
            var ssqDecoder = Resolve<ISsqDecoder>();
            var chartSets = table.Select((e, i) =>
            {
                var charts = ssqDecoder.Decode(ssqReader.Read(new MemoryStream(e.Data)));

                return new ChartSet
                {
                    Charts = charts,
                    Metadata = new Metadata
                    {
                        [ChartTag.MusicTag] = $"{i:D4}.wav",
                        [ChartTag.OffsetTag] = $"{(decimal) -charts.First()[NumericData.LinearOffset]}"
                    }
                };
            }).ToList();

            var smEncoder = Resolve<ISmEncoder>();
            var smWriter = Resolve<ISmStreamWriter>();
            var index = 0;

            foreach (var cs in chartSets)
            {
                var commands = smEncoder.Encode(cs);
                using var stream = this.OpenWrite(Path.Combine("ddr-out", "maxusa", $"{index:D4}", $"{index:D4}.sm"));
                smWriter.Write(stream, commands);
                stream.Flush();
                index++;
            }
        }

        [Test]
        [Explicit]
        public void test2()
        {
            using var source = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "us.filedata.bin"), FileMode.Open);
            var remaining = source.Length;
            var reader = new BinaryReader(source);
            var heuristic = new SvagHeuristic(Resolve<IVagStreamReader>());
            var decoder = Resolve<ISvagDecoder>();
            var index = 0;

            while (remaining > 0x800)
            {
                var oldPosition = source.Position;
                var buffer = reader.ReadBytes(0x800);
                var match = heuristic.Match(buffer);
                if (match == null)
                    continue;

                source.Position = oldPosition;
                var svag = heuristic.Read(match, source);
                var decoded = decoder.Decode(svag);
                this.WriteSound(decoded, Path.Combine("ddr-out", "maxusa", $"{index:D4}", $"{index:D4}.wav"));
                index++;
                
                source.Position = oldPosition + 0x800;
            }
        }
    }
}