using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Beatmania.Heuristics;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Converters;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Ddr.Processors;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Gdi.Streamers;
using RhythmCodex.Heuristics;
using RhythmCodex.Infrastructure;
using RhythmCodex.Meta.Models;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Stepmania;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Model;
using RhythmCodex.Stepmania.Streamers;
using RhythmCodex.Tcb.Converters;
using RhythmCodex.Tcb.Streamers;
using RhythmCodex.Vag.Converters;
using RhythmCodex.Vag.Heuristics;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Ddr.Integration;

public class DdrPs2FileDataIntegrationTests : BaseIntegrationFixture
{
    private string ExecutablePath => Path.Combine("I:", "SLES_551.97");
    private string StepDataPath => Path.Combine("I:", "DATA", "IMAGE.DAT");
    private string FileDataPath => Path.Combine("I:", "DATA", "MDB_SN2.DAT");
    private string OutPath => Path.Combine("ddr-out", "sn2eu");
        
    // private string ExecutablePath => Path.Combine("I:", "SLES_543.41");
    // private string StepDataPath => Path.Combine("I:", "DATA", "IMAGE.DAT");
    // private string FileDataPath => Path.Combine("I:", "DATA", "MDB_SN1.DAT");
    // private string OutPath => Path.Combine("ddr-out", "sneu");
        
    // private string ExecutablePath => Path.Combine("I:", "SLES_515.10");
    // private string FileDataPath => Path.Combine("I:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("I:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "megamixeu");
        
    // private string ExecutablePath => Path.Combine("I:", "SLES_532.18");
    // private string FileDataPath => Path.Combine("I:", "DATA", "FILEDT03.BIN");
    // private string StepDataPath => Path.Combine("I:", "DATA", "FILEDT03.BIN");
    // private string OutPath => Path.Combine("ddr-out", "maxeu");
        
    // private string ExecutablePath => Path.Combine("K:", "SLES_525.98");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "fusioneu");

    // private string ExecutablePath => Path.Combine("K:", "SLES_517.57");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "fevereu");

    // private string ExecutablePath => Path.Combine("K:", "SLPM_621.54");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "maxjpn");

    // private string ExecutablePath => Path.Combine("K:", "SLUS_204.37");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "maxusa");

    // private string ExecutablePath => Path.Combine("K:", "SLPM_652.77");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "max2jpn");
        
    // private string ExecutablePath => Path.Combine("K:", "SLUS_207.11");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "max2usa");

    // private string ExecutablePath => Path.Combine("K:", "SLPM_653.58");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "extremejpn");

    // private string ExecutablePath => Path.Combine("K:", "SLUS_209.16");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "extremeusa");
        
    // private string ExecutablePath => Path.Combine("K:", "SLPM_624.27");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "pcjpn");

    // private string ExecutablePath => Path.Combine("K:", "SLPM_657.75");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // private string OutPath => Path.Combine("ddr-out", "festivaljpn");

    // private string ExecutablePath => Path.Combine("K:", "SLPM_662.42");
    // //private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // //private string FileDataPath => Path.Combine("K:", "DATA", "FILEDT02.BIN");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDT03.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDT03.BIN");
    // private string OutPath => Path.Combine("ddr-out", "strikejpn");
        
    // private string ExecutablePath => Path.Combine("K:", "SLUS_211.74");
    // //private string FileDataPath => Path.Combine("K:", "DATA", "FILEDATA.BIN");
    // //private string FileDataPath => Path.Combine("K:", "DATA", "FILEDT02.BIN");
    // //private string FileDataPath => Path.Combine("K:", "DATA", "FILEDT03.BIN");
    // private string FileDataPath => Path.Combine("K:", "DATA", "FILEDT04.BIN");
    // private string StepDataPath => Path.Combine("K:", "DATA", "FILEDT04.BIN");
    // private string OutPath => Path.Combine("ddr-out", "extreme2usa");

    // private string StepDataPath => Path.Combine("K:", "DATA", "IMAGE.DAT");
    // private string FileDataPath => Path.Combine("K:", "DATA", "MDB_SN1.DAT");
    // private string ExecutablePath => Path.Combine("K:", "SLUS_213.77");
    // private string OutPath => Path.Combine("ddr-out", "snusa");

    // private string StepDataPath => Path.Combine("K:", "DATA", "IMAGE.DAT");
    // private string FileDataPath => Path.Combine("K:", "DATA", "MDB_SN1.DAT");
    // private string ExecutablePath => Path.Combine("K:", "SLPM_666.09");
    // private string OutPath => Path.Combine("ddr-out", "snjpn");
        
    // private string StepDataPath => Path.Combine("K:", "DATA", "IMAGE.DAT");
    // private string FileDataPath => Path.Combine("K:", "DATA", "MDB_SN1.DAT");
    // private string ExecutablePath => Path.Combine("K:", "SLUS_216.08");
    // private string OutPath => Path.Combine("ddr-out", "sn2usa");

    // private string StepDataPath => Path.Combine("K:", "DATA", "IMAGE.DAT");
    // private string FileDataPath => Path.Combine("K:", "DATA", "MDB_SN2.DAT");
    // private string ExecutablePath => Path.Combine("K:", "SLPM_669.30");
    // private string OutPath => Path.Combine("ddr-out", "sn2jpn");

    // private string StepDataPath => Path.Combine("K:", "DATA", "IMAGE.DAT");
    // private string FileDataPath => Path.Combine("K:", "DATA", "MDB_X1.DAT");
    // private string ExecutablePath => Path.Combine("K:", "SLUS_217.67");
    // private string OutPath => Path.Combine("ddr-out", "xusa");

    // private string StepDataPath => Path.Combine("K:", "DATA", "IMAGE.DAT");
    // private string FileDataPath => Path.Combine("K:", "DATA", "MDB_X1.DAT");
    // private string ExecutablePath => Path.Combine("K:", "SLPM_550.90");
    // private string OutPath => Path.Combine("ddr-out", "xjpn");

    // private string StepDataPath => Path.Combine("K:", "DATA", "IMAGE.DAT");
    // private string FileDataPath => Path.Combine("K:", "DATA", "MDB_X1.DAT");
    // private string ExecutablePath => Path.Combine("K:", "SLUS_219.17");
    // private string OutPath => Path.Combine("ddr-out", "x2usa");
        
    [Test]
    [Explicit]
    public void Test0()
    {
        using var mdSource = new FileStream(ExecutablePath, FileMode.Open, FileAccess.Read);
        var metadataDecoder = Resolve<IDdrPs2MetadataTableStreamReader>();
        var dbDecoder = Resolve<IDdrPs2DatabaseDecoder>();
        var rawMetaDatas = metadataDecoder.Get(mdSource, mdSource.Length).Select(dbDecoder.Decode).ToList();
        foreach (var md in rawMetaDatas)
            TestContext.WriteLine($"{md}");
    }

    [Test]
    [TestCase(0x292000)]
    [TestCase(0x472000)]
    [TestCase(0x6ED800)]
    [TestCase(0x715000)]
    [Explicit]
    public void Test_Export_TCB_Unbound(long offset)
    {
        var tableReader = Resolve<IDdrPs2FileDataTableChunkStreamReader>();
        using var tableSource = new FileStream(StepDataPath, FileMode.Open, FileAccess.Read) {Position = offset};
        var table = tableReader.GetUnbound(tableSource);
            
        var tableDecoder = Resolve<IDdrPs2FileDataUnboundTableDecoder>();
        var decodedTable = tableDecoder.Decode(table);

        var imageReader = Resolve<ITcbStreamReader>();
        var imageDecoder = Resolve<ITcbImageDecoder>();
        var pngEncoder = Resolve<IPngStreamWriter>();

        foreach (var entry in decodedTable)
        {
            var name = $"{entry.Index:D4}";
            using var imageStream = new MemoryStream(entry.Data);
            var tcb = imageReader.Read(imageStream);
            if (tcb == null)
                continue;
            var bitmap = imageDecoder.Decode(tcb);
            using var outStream = this.OpenWrite(Path.Combine($"{offset:X8}", $"{name}.png"));
            pngEncoder.Write(outStream, bitmap);
            outStream.Flush();
        }
    }

    [Test]
    [TestCase(0x7DC000)]
    [TestCase(0x949800)]
    [Explicit]
    public void Test_Export_TCB_Bound(long offset)
    {
        using var tableSource = new FileStream(StepDataPath, FileMode.Open, FileAccess.Read) {Position = offset};
        var reader = new BinaryReader(tableSource);
        var count = reader.ReadInt32();
        var offsets = Enumerable.Range(0, count).Select(_ => reader.ReadInt32()).ToArray();
        var lengths = Enumerable.Range(0, count).Select(_ => reader.ReadInt32()).ToArray();

        var decomp = Resolve<IBemaniLzDecoder>();
        var imageReader = Resolve<ITcbStreamReader>();
        var imageDecoder = Resolve<ITcbImageDecoder>();
        var pngEncoder = Resolve<IPngStreamWriter>();
            
        for (var i = 0; i < count; i++)
        {
            var name = $"{i:D4}";
            tableSource.Position = offset + offsets[i];
            var file = decomp.Decode(tableSource);
            var imageStream = new MemoryStream(file);
            var tcb = imageReader.Read(imageStream);
            if (tcb == null)
                continue;
            var bitmap = imageDecoder.Decode(tcb);
            using var outStream = this.OpenWrite(Path.Combine($"{offset:X8}", $"{name}.png"));
            pngEncoder.Write(outStream, bitmap);
            outStream.Flush();
        }

        // var tableReader = Resolve<IDdrPs2FileDataTableChunkStreamReader>();
        // using var tableSource = new FileStream(StepDataPath, FileMode.Open, FileAccess.Read) {Position = offset};
        // var table = tableReader.GetBound(tableSource);
        // this.WriteFile(table.Data, Path.Combine($"{offset:X8}", "table.out"));
        //
        // var tableDecoder = Resolve<IDdrPs2FileDataBoundTableDecoder>();
        // var decodedTable = tableDecoder.Decode(table);
        //
        // var imageReader = Resolve<ITcbStreamReader>();
        // var imageDecoder = Resolve<ITcbImageDecoder>();
        // var pngEncoder = Resolve<IPngStreamWriter>();
        //
        // foreach (var entry in decodedTable)
        // {
        //     var name = $"{entry.Index:D4}";
        //     using var imageStream = new MemoryStream(entry.Data);
        //     this.WriteFile(entry.Data, Path.Combine($"{offset:X8}", $"{name}.bin"));
        //     var tcb = imageReader.Read(imageStream);
        //     if (tcb == null)
        //         continue;
        //     var bitmap = imageDecoder.Decode(tcb);
        //     using var outStream = this.OpenWrite(Path.Combine($"{offset:X8}", $"{name}.png"));
        //     pngEncoder.Write(outStream, bitmap);
        //     outStream.Flush();
        // }
    }

    [Test]
    [Explicit]
    public void Test_Export_Filedata_TCB()
    {
        using var source = new FileStream(FileDataPath, FileMode.Open, FileAccess.Read);
        var sourceReader = new BinaryReader(source);
        var imageReader = Resolve<ITcbStreamReader>();
        var imageDecoder = Resolve<ITcbImageDecoder>();
        var pngEncoder = Resolve<IPngStreamWriter>();
        var decompressor = Resolve<IBemaniLzDecoder>();

        var offset = 0L;

        while (offset < source.Length)
        {
            source.Position = offset;
            var id = sourceReader.ReadInt32();
            var success = false;
            var decompress = false;
                
            if (id == 0x00424354)
            {
                success = true;
            }
            else if (((id >> 8) & 0xFFFFFF) == 0x00424354)
            {
                success = true;
                decompress = true;
            }

            if (!success)
            {
                offset += 0x800;
                continue;
            }

            source.Position = offset;
            Stream imageStream = source;

            if (decompress)
            {
                var decompressed = decompressor.Decode(source);
                imageStream = new MemoryStream(decompressed);
            }
                
            var name = $"{offset:X8}";
            var tcb = imageReader.Read(imageStream);
            var bitmap = imageDecoder.Decode(tcb);
            using var outStream = this.OpenWrite(Path.Combine($"searched", $"{name}.png"));
            pngEncoder.Write(outStream, bitmap);
            outStream.Flush();
                
            offset += 0x800;
        }
    }
        
    [Test]
    [Explicit]
    public void Test_Export_SN_WAV()
    {
        using var mdSource = new FileStream(ExecutablePath, FileMode.Open, FileAccess.Read);
        var metadataDecoder = Resolve<IDdrPs2MetadataTableStreamReader>();
        var dbDecoder = Resolve<IDdrPs2DatabaseDecoder>();
        var rawMetaDatas = metadataDecoder.Get(mdSource, mdSource.Length).Select(dbDecoder.Decode)
            .OrderBy(x => x.AudioTrack).ToList();
        foreach (var md in rawMetaDatas)
            TestContext.WriteLine($"{md}");
        var firstSongIndex = rawMetaDatas.Where(x => x.AudioTrack > 0).Select(x => x.AudioTrack).Min();
            
            
            
        using var source = new FileStream(FileDataPath, FileMode.Open, FileAccess.Read);
        var vagDecoder = Resolve<VagDecoder>();
            
        // var metadataDecorator = Resolve<IDdrMetadataDecorator>();
        // var ssqDecoder = Resolve<ISsqDecoder>();
        // var smEncoder = Resolve<ISmEncoder>();
        // var smWriter = Resolve<ISmStreamWriter>();

        var soundIndex = 0;
        // var chartIndex = 0;
        var max = source.Length - 0x800;
        // var ssqHeuristic = Resolve<SsqHeuristic>();
        var bgmHeuristic = Resolve<BeatmaniaPs2NewBgmHeuristic>();
        var cache = new CachedStream(source);

        for (var offset = 0L; offset < max; offset += 0x800)
        {
            source.Position = offset;
            cache.Reset();
                
            // if (ssqHeuristic.Match(cache) is HeuristicResult ssqResult)
            // {
            //     cache.Rewind();
            //     var charts = ssqDecoder.Decode(ssqHeuristic.Read(ssqResult, cache));
            //     // var idMd = rawMetaDatas.FirstOrDefault(md => md.InternalId == chartIndex + 1);
            //
            //     var chartSet = new ChartSet
            //     {
            //         Charts = charts,
            //         Metadata = new Metadata()
            //     };
            //
            //     // metadataDecorator.Decorate(chartSet, idMd, new MetadataDecoratorFileExtensions());
            //     var commands = smEncoder.Encode(chartSet);
            //     using var stream = this.OpenWrite(Path.Combine(OutPath, $"{chartIndex:D4}.sm"));
            //     smWriter.Write(stream, commands);
            //     stream.Flush();
            //     chartIndex++;
            //     
            //     continue;
            // }
            // cache.Rewind();

            if (bgmHeuristic.Match(cache) is HeuristicResult bgmResult)
            {
                cache.Rewind();

                //var idMd = rawMetaDatas.FirstOrDefault(md => md.AudioTrack == soundIndex + firstSongIndex);
                //var idMd = soundIndex >= rawMetaDatas.Count ? null : rawMetaDatas[soundIndex];
                var fileName = $"{soundIndex:D4}"; //idMd?.Id ?? $"{soundIndex:D4}";
                var vag = bgmHeuristic.Read(bgmResult, cache);
                var decoded = vagDecoder.Decode(vag);
                this.WriteSound(decoded, Path.Combine(OutPath, $"{fileName}.wav"));
                soundIndex++;
            }
        }
    }
        
    [Test]
    [Explicit]
    public void Test_Export_SM()
    {
        using var mdSource = new FileStream(ExecutablePath, FileMode.Open, FileAccess.Read);
        var metadataDecoder = Resolve<IDdrPs2MetadataTableStreamReader>();
        var dbDecoder = Resolve<IDdrPs2DatabaseDecoder>();
        var rawMetaDatas = metadataDecoder.Get(mdSource, mdSource.Length).Select(dbDecoder.Decode).ToList();

        var metadataDb = Resolve<IDdrMetadataDatabase>();

        using var source = new FileStream(StepDataPath, FileMode.Open, FileAccess.Read);
        var streamer = Resolve<IDdrPs2FileDataStepStreamReader>();
        var output = streamer.Read(source, source.Length);

        var tableDecoder = Resolve<IDdrPs2FileDataUnboundTableDecoder>();
        var table = tableDecoder.Decode(output);

        var metadataDecorator = Resolve<IDdrMetadataDecorator>();
        var ssqReader = Resolve<ISsqStreamReader>();
        var ssqDecoder = Resolve<ISsqDecoder>();
        var chartSets = table.Select((e, i) =>
        {
            var charts = ssqDecoder.Decode(ssqReader.Read(new MemoryStream(e.Data)));
            var idMd = rawMetaDatas.FirstOrDefault(md => md.InternalId == i + 1);

            var newMd = new Metadata();
            var chartSet = new ChartSet
            {
                Charts = charts,
                Metadata = newMd
            };
                
            metadataDecorator.Decorate(chartSet, idMd, new MetadataDecoratorFileExtensions());
                
            var fullMd = idMd != null ? metadataDb.GetByCode(idMd.Id) : null;
            newMd[StringData.Comment] = idMd?.Id;
            newMd[ChartTag.TitleTag] = fullMd?.Title ?? fullMd?.TitleRoman ?? fullMd?.TitleLocal ?? newMd[ChartTag.TitleTag];
            newMd[ChartTag.SubTitleTag] = fullMd?.Subtitle ?? fullMd?.SubtitleRoman ?? fullMd?.SubtitleLocal ?? newMd[ChartTag.SubTitleTag];
            newMd[ChartTag.ArtistTag] = fullMd?.Artist ?? fullMd?.ArtistRoman ?? fullMd?.ArtistLocal ?? newMd[ChartTag.ArtistTag];
            newMd[ChartTag.TitleTranslitTag] = fullMd?.TitleRoman ?? fullMd?.TitleLocal ?? newMd[ChartTag.TitleTranslitTag];
            newMd[ChartTag.SubTitleTranslitTag] = fullMd?.SubtitleRoman ?? fullMd?.SubtitleLocal ?? newMd[ChartTag.SubTitleTranslitTag];
            newMd[ChartTag.ArtistTranslitTag] = fullMd?.ArtistRoman ?? fullMd?.ArtistLocal ?? newMd[ChartTag.ArtistTranslitTag];
                
            return chartSet;
        }).ToList();

        var smEncoder = Resolve<ISmEncoder>();
        var smWriter = Resolve<ISmStreamWriter>();
        var index = 0;

        foreach (var cs in chartSets)
        {
            var commands = smEncoder.Encode(cs);
            using var stream = this.OpenWrite(Path.Combine(OutPath, cs.Metadata[StringData.Comment] ?? $"{index:D4}", $"{index:D4}.sm"));
            smWriter.Write(stream, commands);
            stream.Flush();
            index++;
        }
    }

    [Test]
    [Explicit]
    public void Test_Export_WAV()
    {
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
            remaining -= 0x800;
        }
    }
}