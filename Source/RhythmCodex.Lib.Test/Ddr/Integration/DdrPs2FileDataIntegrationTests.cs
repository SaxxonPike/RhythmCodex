using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Processors;
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
        public void Test0()
        {
            using var mdSource = new FileStream(Path.Combine("L:", "SLUS_207.11"), FileMode.Open, FileAccess.Read);
            var metadataDecoder = Resolve<IDdrPs2MetadataTableStreamReader>();
            var dbDecoder = Resolve<IDdrPs2DatabaseDecoder>();
            var rawMetaDatas = metadataDecoder.Get(mdSource, mdSource.Length).Select(dbDecoder.Decode).ToList();
        }
        
        [Test]
        [Explicit]
        public void test1()
        {
            using var mdSource = new FileStream(Path.Combine("I:", "SLUS_209.16"), FileMode.Open, FileAccess.Read);
            var metadataDecoder = Resolve<IDdrPs2MetadataTableStreamReader>();
            var dbDecoder = Resolve<IDdrPs2DatabaseDecoder>();
            var rawMetaDatas = metadataDecoder.Get(mdSource, mdSource.Length).Select(dbDecoder.Decode).ToList();

            using var source = new FileStream(Path.Combine("I:", "data", "filedata.bin"), FileMode.Open, FileAccess.Read);
            var streamer = Resolve<IDdrPs2FileDataStepStreamReader>();
            var output = streamer.Read(source, source.Length);

            var tableDecoder = Resolve<IDdrPs2FileDataTableDecoder>();
            var table = tableDecoder.Decode(output);

            var metadataDecorator = Resolve<IDdrPs2MetadataDecorator>();
            var ssqReader = Resolve<ISsqStreamReader>();
            var ssqDecoder = Resolve<ISsqDecoder>();
            var chartSets = table.Select((e, i) =>
            {
                var charts = ssqDecoder.Decode(ssqReader.Read(new MemoryStream(e.Data)));
                var idMd = rawMetaDatas.FirstOrDefault(md => md.InternalId == i + 1);
                var id = idMd?.Id ?? $"{i:D4}";
                
                var chartSet = new ChartSet
                {
                    Charts = charts,
                    Metadata = new Metadata
                    {
                        [ChartTag.TitleTag] = id,
                        [ChartTag.MusicTag] = $"{id}.wav",
                        [ChartTag.OffsetTag] = $"{(decimal) -charts.First()[NumericData.LinearOffset]}",
                        [ChartTag.DisplayBpmTag] = $"{idMd.MinBpm}:{idMd.MaxBpm}",
                        [ChartTag.BannerTag] = $"{id}_th.png",
                        [ChartTag.BackgroundTag] = $"{id}_bk.png"
                    }
                };
                
                metadataDecorator.Decorate(chartSet, idMd);
                return chartSet;
            }).ToList();

            var smEncoder = Resolve<ISmEncoder>();
            var smWriter = Resolve<ISmStreamWriter>();
            var index = 6;

            foreach (var cs in chartSets)
            {
                var commands = smEncoder.Encode(cs);
                using var stream = this.OpenWrite(Path.Combine("ddr-out", "extreme", cs.Metadata[ChartTag.TitleTag], $"{index:D4}.sm"));
                smWriter.Write(stream, commands);
                stream.Flush();
                index++;
            }
        }

        [Test]
        [Explicit]
        public void test2()
        {
            using var mdSource = new FileStream(Path.Combine("I:", "SLUS_209.16"), FileMode.Open, FileAccess.Read);
            var md = Resolve<IDdrPs2MetadataTableStreamReader>();
            var metaDatas = md.Get(mdSource, mdSource.Length);

            using var source = new FileStream(Path.Combine("I:", "data", "filedata.bin"), FileMode.Open, FileAccess.Read);
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
                this.WriteSound(decoded, Path.Combine("ddr-out", "extreme", $"{index:D4}.wav"));
                index++;
                
                source.Position = oldPosition + 0x800;
            }
        }
    }
}