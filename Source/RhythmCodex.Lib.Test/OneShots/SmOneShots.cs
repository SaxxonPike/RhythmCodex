using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Infrastructure;
using RhythmCodex.Stepmania.Converters;
using RhythmCodex.Stepmania.Model;
using RhythmCodex.Stepmania.Streamers;

namespace RhythmCodex.OneShots
{
    [TestFixture]
    public class SmOneShots : BaseIntegrationFixture
    {
        [Test]
        [Explicit("This is a tool, not a test")]
        [TestCase("c:\\stepmania\\songs\\DDR SOLO 2000", 0.002)]
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