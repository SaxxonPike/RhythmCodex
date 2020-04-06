using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Models;
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
        // private string ExecutablePath => Path.Combine("K:", "SLUS_207.11");
        // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
        // private string OutPath => Path.Combine("ddr-out", "max2usa");

        private string ExecutablePath => Path.Combine("K:", "SLUS_204.37");
        private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
        private string OutPath => Path.Combine("ddr-out", "maxusa");
        
        [Test]
        [Explicit]
        public void Test0()
        {
            using var mdSource = new FileStream(ExecutablePath, FileMode.Open, FileAccess.Read);
            var metadataDecoder = Resolve<IDdrPs2MetadataTableStreamReader>();
            var dbDecoder = Resolve<IDdrPs2DatabaseDecoder>();
            var rawMetaDatas = metadataDecoder.Get(mdSource, mdSource.Length).Select(dbDecoder.Decode).ToList();
        }
        
        [Test]
        [Explicit]
        public void Test_Export_SM()
        {
            using var mdSource = new FileStream(ExecutablePath, FileMode.Open, FileAccess.Read);
            var metadataDecoder = Resolve<IDdrPs2MetadataTableStreamReader>();
            var dbDecoder = Resolve<IDdrPs2DatabaseDecoder>();
            var rawMetaDatas = metadataDecoder.Get(mdSource, mdSource.Length).Select(dbDecoder.Decode).ToList();

            using var source = new FileStream(FileDataPath, FileMode.Open, FileAccess.Read);
            var streamer = Resolve<IDdrPs2FileDataStepStreamReader>();
            var output = streamer.Read(source, source.Length);

            var tableDecoder = Resolve<IDdrPs2FileDataTableDecoder>();
            var table = tableDecoder.Decode(output);

            var metadataDecorator = Resolve<IDdrMetadataDecorator>();
            var ssqReader = Resolve<ISsqStreamReader>();
            var ssqDecoder = Resolve<ISsqDecoder>();
            var chartSets = table.Select((e, i) =>
            {
                var charts = ssqDecoder.Decode(ssqReader.Read(new MemoryStream(e.Data)));
                var idMd = rawMetaDatas.FirstOrDefault(md => md.InternalId == i + 1);
                
                var chartSet = new ChartSet
                {
                    Charts = charts,
                    Metadata = new Metadata()
                };
                
                metadataDecorator.Decorate(chartSet, idMd, new MetadataDecoratorFileExtensions());
                return chartSet;
            }).ToList();

            var smEncoder = Resolve<ISmEncoder>();
            var smWriter = Resolve<ISmStreamWriter>();
            var index = 0;

            foreach (var cs in chartSets)
            {
                var commands = smEncoder.Encode(cs);
                using var stream = this.OpenWrite(Path.Combine(OutPath, cs.Metadata[ChartTag.TitleTag], $"{index:D4}.sm"));
                smWriter.Write(stream, commands);
                stream.Flush();
                index++;
            }
        }

        [Test]
        [Explicit]
        public void Test_Export_WAV()
        {
            using var mdSource = new FileStream(ExecutablePath, FileMode.Open, FileAccess.Read);
            var md = Resolve<IDdrPs2MetadataTableStreamReader>();
            var metaDatas = md.Get(mdSource, mdSource.Length);

            using var source = new FileStream(FileDataPath, FileMode.Open, FileAccess.Read);
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
                this.WriteSound(decoded, Path.Combine(OutPath, $"{index:D4}.wav"));
                index++;
                
                source.Position = oldPosition + 0x800;
            }
        }
    }
}