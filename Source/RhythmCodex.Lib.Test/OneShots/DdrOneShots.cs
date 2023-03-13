using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using RhythmCodex.Arc.Converters;
using RhythmCodex.Arc.Streamers;
using RhythmCodex.Compression;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Dds.Converters;
using RhythmCodex.Dds.Streamers;
using RhythmCodex.Extensions;
using RhythmCodex.Gdi.Streamers;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Ssq.Converters;
using RhythmCodex.Ssq.Streamers;
using RhythmCodex.Stepmania;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Model;
using RhythmCodex.Stepmania.Streamers;
using RhythmCodex.Xact.Converters;
using RhythmCodex.Xact.Streamers;

namespace RhythmCodex.OneShots
{
    [TestFixture]
    public class DdrOneShots : BaseIntegrationFixture
    {
        [Test]
        [Explicit("This is a tool, not a test")]
        [TestCase("Z:\\Bemani\\Dance Dance Revolution 573\\ddr4mps CARD.DAT", 0)]
        public void GetAcDb(string path, int offset)
        {
            var decoder = Resolve<BemaniLzDecoder>();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var data = decoder.Decode(stream);
            this.WriteFile(data, "db.bin");
        }
        
        [Test]
        [Explicit("This is a tool, not a test")]
        [TestCase("K:\\SLUS_219.17", 0x1AF904, 0x1E7550, 0x2E73D0)]
        public void GenerateEditDb(string executable, int recordOffset, int stringOffset, int stringBase)
        {
            var output = new StringBuilder();
            output.AppendLine("<?xml version=\"1.0\"?>");
            output.AppendLine("<DdrEditDatabase>");
            
            using var stream = new FileStream(executable, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(stream);

            string ReadZeroString(int offs)
            {
                var realOffs = offs - stringBase + stringOffset;
                if (realOffs < 0)
                    return null;
                stream.Position = realOffs;
                var result = new List<byte>();
                while (true)
                {
                    var b = reader.ReadByte();
                    if (b == 0)
                        break;
                    result.Add(b);
                }

                return result.ToArray().GetString();
            }

            var offset = recordOffset;
            while (true)
            {
                stream.Position = offset;
                var id = reader.ReadInt32();
                if (id == 0)
                    break;

                var letterOffset = reader.ReadInt32();
                var titleOffset = reader.ReadInt32();
                var altTitleOffset = reader.ReadInt32();
                var artistOffset = reader.ReadInt32();

                var letterString = ReadZeroString(letterOffset);
                var titleString = ReadZeroString(titleOffset);
                var altTitleString = ReadZeroString(altTitleOffset);
                var artistString = ReadZeroString(artistOffset);

                output.AppendLine($"    <Song Id=\"{id}\">");
                if (letterString != null)
                    output.AppendLine($"        <Code>{letterString}</Code>");
                if (titleString != null)
                    output.AppendLine($"        <Title>{titleString}</Title>");
                if (altTitleString != null)
                    output.AppendLine($"        <ShortTitle>{altTitleString}</ShortTitle>");
                if (artistString != null)
                    output.AppendLine($"        <Artist>{artistString}</Artist>");
                output.AppendLine("    </Song>");
                
                offset += 0x14;
            }
            
            output.AppendLine("</DdrEditDatabase>");

            using var outStream = this.OpenWrite("editdb.xml");
            var writer = new StreamWriter(outStream);
            writer.Write(output);
            writer.Flush();
        }
        
        [Test]
        [Explicit("This is a tool, not a test")]
        [TestCase(@"\\tamarat\ddr\MDX-001-2018102200\contents\data", true, true, true)]
        public void ConvertModernDdrData(string basePath, bool enableSound, bool enableSteps, bool enableGraphics)
        {
            var ssqPath = Path.Combine(basePath, "mdb_apx", "ssq");
            var jacketPath = Path.Combine(basePath, "arc", "jacket");
            var soundPath = Path.Combine(basePath, "sound", "win", "dance");
            var startupPath = Path.Combine(basePath, "arc", "startup.arc");

            var arcReader = Resolve<IArcStreamReader>();
            var arcConverter = Resolve<IArcFileConverter>();
            var musicDbReader = Resolve<IMusicDbXmlStreamReader>();
            var xwbReader = Resolve<IXwbStreamReader>();
            var xwbDecoder = Resolve<IXwbDecoder>();
            var ssqReader = Resolve<ISsqStreamReader>();
            var ssqDecoder = Resolve<ISsqDecoder>();
            var ddsReader = Resolve<IDdsStreamReader>();
            var ddsDecoder = Resolve<IDdsBitmapDecoder>();
            var smEncoder = Resolve<ISmEncoder>();
            var smWriter = Resolve<ISmStreamWriter>();
            var riffEncoder = Resolve<IRiffPcm16SoundEncoder>();
            var riffWriter = Resolve<IRiffStreamWriter>();
            var pngWriter = Resolve<IPngStreamWriter>();

            using var startupArc = File.OpenRead(startupPath);
            var startupFiles = arcReader.Read(startupArc);
            var musicDb = startupFiles.Single(x =>
                x.Name.Split('/').Last().Equals("musicdb.xml", StringComparison.CurrentCultureIgnoreCase));
            musicDb = arcConverter.Decompress(musicDb);
            using var musicDbStream = new MemoryStream(musicDb.Data);
            var musicDbEntries = musicDbReader.Read(musicDbStream).AsList();

            foreach (var metadata in musicDbEntries)
            {
                var outFolder = Path.Combine("ddr-out", metadata.BaseName);
                if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    outFolder)))
                    continue;

                // Get sounds
                var xwbFileName = Path.Combine(soundPath, $"{metadata.BaseName}.xwb");
                using var xwbFileStream = !enableSound || !File.Exists(xwbFileName) ? null : File.OpenRead(xwbFileName);
                var xwb = xwbFileStream == null ? null : xwbReader.Read(xwbFileStream).AsList();
                var xwbSongSound = !enableSound ? null : xwb?.SingleOrDefault(x =>
                    x.Name?.Equals(metadata.BaseName, StringComparison.OrdinalIgnoreCase) ?? false);
                var xwbPreviewSound = !enableSound ? null : xwb?.SingleOrDefault(x =>
                    x.Name?.Equals($"{metadata.BaseName}_s", StringComparison.OrdinalIgnoreCase) ?? false);
                if (enableSound && xwbSongSound == null)
                {
                    if (xwb?.Count == 2)
                    {
                        var xwbBySize = xwb.OrderBy(x => x.Data.Length).ToList();
                        xwbPreviewSound = xwbBySize[0];
                        xwbSongSound = xwbBySize[1];
                    }
                    else
                    {
                        TestContext.WriteLine($"NOT sure what to do with sound on {metadata.BaseName}");
                    }
                }

                // Get steps
                var ssqFileName = Path.Combine(ssqPath, $"{metadata.BaseName}.ssq");
                using var ssqFileStream = !enableSteps || !File.Exists(ssqFileName) ? null : File.OpenRead(ssqFileName);
                var ssq = ssqFileStream == null ? null : ssqReader.Read(ssqFileStream);
                var charts = ssq == null ? null : ssqDecoder.Decode(ssq);

                // Get graphics
                var arcFileName = Path.Combine(jacketPath, $"{metadata.BaseName}_jk.arc");
                using var arcFileStream = !enableGraphics || !File.Exists(arcFileName) ? null : File.OpenRead(arcFileName);
                var arc = arcFileStream == null ? null : arcReader.Read(arcFileStream);
                var arcJacket = arc?.SingleOrDefault(x =>
                    x.Name?.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).Last()
                        .Equals($"{metadata.BaseName}_jk.dds", StringComparison.OrdinalIgnoreCase) ?? false);
                using var ddsStream = arcJacket == null
                    ? null
                    : new MemoryStream(arcConverter.Decompress(arcJacket).Data);
                var jacket = ddsStream == null
                    ? null
                    : ddsDecoder.Decode(ddsReader.Read(ddsStream, (int) ddsStream.Length));
                
                // Convert sounds
                var songSound = xwbSongSound == null ? null : xwbDecoder.Decode(xwbSongSound);
                var previewSound = xwbPreviewSound == null ? null : xwbDecoder.Decode(xwbPreviewSound);
                using var songStream = songSound == null ? null : new MemoryStream();
                using var previewStream = previewSound == null ? null : new MemoryStream();
                if (songStream != null)
                    riffWriter.Write(songStream, riffEncoder.Encode(songSound));
                if (previewStream != null)
                    riffWriter.Write(previewStream, riffEncoder.Encode(previewSound));
                
                // Convert steps
                var smMetadata = new Metadata
                {
                    [ChartTag.TitleTag] = metadata.Title,
                    [ChartTag.ArtistTag] = metadata.Artist,
                    [ChartTag.MusicTag] = $"{metadata.BaseName}.ogg",
                    [ChartTag.BannerTag] = $"{metadata.BaseName}-jacket.png",
                    [ChartTag.DisplayBpmTag] = metadata.BpmMin != null ? $"{metadata.BpmMin}:{metadata.BpmMax}" : $"{metadata.BpmMax}",
                    [ChartTag.PreviewTag] = $"{metadata.BaseName}-preview.ogg"
                };
                var sm = charts == null ? null : smEncoder.Encode(new ChartSet {Charts = charts, Metadata = smMetadata});
                using var smStream = sm == null ? null : new MemoryStream();
                if (smStream != null)
                    smWriter.Write(smStream, sm);
                
                // Convert graphics
                using var jacketPngStream = jacket == null ? null : new MemoryStream();
                if (jacketPngStream != null)
                    pngWriter.Write(jacketPngStream, jacket);
                
                // Write sounds
                if (songStream != null)
                {
                    using var output = this.OpenWrite(Path.Combine(outFolder, $"{metadata.BaseName}.wav"));
                    songStream.WriteTo(output);
                    output.Flush();
                }
                if (previewStream != null)
                {
                    using var output = this.OpenWrite(Path.Combine(outFolder, $"{metadata.BaseName}-preview.wav"));
                    previewStream.WriteTo(output);
                    output.Flush();
                }
                
                // Write steps
                if (smStream != null)
                {
                    using var output = this.OpenWrite(Path.Combine(outFolder, $"{metadata.BaseName}.sm"));
                    smStream.WriteTo(output);
                    output.Flush();
                }
                
                // Write graphics
                if (jacketPngStream != null)
                {
                    using var output = this.OpenWrite(Path.Combine(outFolder, $"{metadata.BaseName}-jacket.png"));
                    jacketPngStream.WriteTo(output);
                    output.Flush();
                }
            }
        }
    }
}