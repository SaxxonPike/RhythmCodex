using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using RhythmCodex.Arc.Converters;
using RhythmCodex.Arc.Streamers;
using RhythmCodex.Ddr.Streamers;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania;
using RhythmCodex.Stepmania.Model;
using RhythmCodex.Stepmania.Streamers;

namespace RhythmCodex.OneShots
{
    [TestFixture]
    public class SmOneShots : BaseIntegrationFixture
    {
        [Test]
        [Explicit("This is a tool, not a test")]
        [TestCase(@"\\tamarat\ddr\MDX-001-2018102200\contents\data")]
        public void ConvertModernDdrData(string basePath)
        {
            var ssqPath = Path.Combine(basePath, "mdb_apx", "ssq");
            var jacketPath = Path.Combine(basePath, "arc", "jacket");
            var soundPath = Path.Combine(basePath, "sound", "win", "dance");
            var startupPath = Path.Combine(basePath, "arc", "startup.arc");

            var arcReader = Resolve<IArcStreamReader>();
            var arcConverter = Resolve<IArcFileConverter>();
            var musicDbReader = Resolve<IMusicDbXmlStreamReader>();

            using var startupArc = File.OpenRead(startupPath);
            var startupFiles = arcReader.Read(startupArc);
            var musicDb = startupFiles.Single(x =>
                x.Name.Split('/').Last().Equals("musicdb.xml", StringComparison.CurrentCultureIgnoreCase));
            musicDb = arcConverter.Decompress(musicDb);
            using var musicDbStream = new MemoryStream(musicDb.Data);
            var musicDbEntries = musicDbReader.Read(musicDbStream).AsList();

        }

        [Test]
        [Explicit("This is a tool, not a test")]
        [TestCase(@"C:\StepMania\Songs\DDR 4TH MIX PLUS")]
        public void PopulateMetadataInSmFolders(string path)
        {
            var smReader = Resolve<ISmStreamReader>();
            var smWriter = Resolve<ISmStreamWriter>();

            var files = Directory.GetFiles(path, "*.sm", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                IList<Command> commands;
                using (var stream = File.OpenRead(file))
                    commands = smReader.Read(stream).ToList();

                var smPath = Path.GetDirectoryName(file);
                var images = Directory.GetFiles(smPath, "*.png").ToDictionary(f => f, Image.FromFile);
                var musics = Directory.GetFiles(smPath, "*.mp3").ToDictionary(f => f, f => new FileInfo(f));

                // replace banner
                var command = GetOrCreateCommand(commands, ChartTag.BannerTag);
                command.Values.Clear();
                var bannerImage = images.FirstOrDefault(i =>
                    (i.Value.Width == 256 && i.Value.Height == 80) ||
                    (i.Value.Width == 192 && i.Value.Height == 55));
                if (bannerImage.Value != null)
                    command.Values.Add(Path.GetFileName(bannerImage.Key));

                // replace bg
                command = GetOrCreateCommand(commands, ChartTag.BackgroundTag);
                command.Values.Clear();
                var bgImage = images.FirstOrDefault(i =>
                    (i.Value.Width == 320 && i.Value.Height == 240) ||
                    (i.Value.Width == 640 && i.Value.Height == 480));
                if (bgImage.Value != null)
                    command.Values.Add(Path.GetFileName(bgImage.Key));

                // replace mp3
                command = GetOrCreateCommand(commands, ChartTag.MusicTag);
                command.Values.Clear();
                var mainMusic = musics.OrderBy(m => m.Value.Length).LastOrDefault();
                if (mainMusic.Value != null)
                    command.Values.Add(Path.GetFileName(mainMusic.Key));

                // replace preview
                command = GetOrCreateCommand(commands, ChartTag.PreviewTag);
                command.Values.Clear();
                if (musics.Count > 1)
                {
                    var prevMusic = musics.OrderBy(m => m.Value.Length).FirstOrDefault();
                    if (prevMusic.Value != null)
                        command.Values.Add(Path.GetFileName(prevMusic.Key));
                }

                // replace file name
                command = GetOrCreateCommand(commands, ChartTag.TitleTag);
                command.Values.Clear();
                command.Values.Add(Path.GetFileNameWithoutExtension(smPath));

                using (var stream = File.Open(file, FileMode.Create, FileAccess.Write))
                {
                    smWriter.Write(stream, commands);
                    stream.Flush();
                }
            }
        }

        private Command GetOrCreateCommand(ICollection<Command> commands, string name)
        {
            var existingCommand = commands.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existingCommand != null) return existingCommand;

            existingCommand = new Command {Name = name, Values = new List<string>()};

            commands.Add(existingCommand);
            return existingCommand;
        }

        [Test]
        [Explicit("This is a tool, not a test")]
        [TestCase(@"C:\StepMania\Songs\DDR 4TH MIX PLUS", -0.033)]
        public void AdjustGapForFolder(string path, double amount)
        {
            var smReader = Resolve<ISmStreamReader>();
            var smWriter = Resolve<ISmStreamWriter>();

            var files = Directory.GetFiles(path, "*.sm", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                IList<Command> commands;
                using (var stream = File.OpenRead(file))
                    commands = smReader.Read(stream).ToList();

                foreach (var offsetCommand in commands.Where(c =>
                    c.Name.Equals("offset", StringComparison.OrdinalIgnoreCase)))
                {
                    var value = offsetCommand.Values.SingleOrDefault();
                    if (value == null)
                    {
                        offsetCommand.Values.Add($"{amount}");
                    }
                    else if (value == string.Empty)
                    {
                        offsetCommand.Values[0] = $"{amount}";
                    }
                    else
                    {
                        var numValue = BigRationalParser.ParseString(offsetCommand.Values.Single());
                        numValue += amount;
                        offsetCommand.Values[0] = $"{(double) numValue}";
                    }
                }

                using (var stream = File.Open(file, FileMode.Create, FileAccess.Write))
                {
                    smWriter.Write(stream, commands);
                    stream.Flush();
                }
            }
        }
    }
}