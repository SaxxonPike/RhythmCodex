using System.Collections.Generic;
using System.Linq;
using RhythmCodex.Cli;
using RhythmCodex.IoC;

namespace RhythmCodex.Gui
{
    [Service]
    public class GuiTasks : IGuiTasks
    {
        private readonly IApp _app;

        public GuiTasks(IApp app)
        {
            _app = app;
        }

        private static string[] CleanArgs(IEnumerable<string> args) =>
            args.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();

        private static void AddOutPath(ICollection<string> args, string outPath)
        {
            if (string.IsNullOrWhiteSpace(outPath))
                return;

            args.Add("-o");
            args.Add(outPath);
        }

        private static void AddFiles(ICollection<string> args, params string[] files)
        {
            var splitFiles = files.SelectMany(f => f.Split('|'));
            foreach (var file in splitFiles.Where(f => !string.IsNullOrWhiteSpace(f)))
                args.Add(file);
        }

        public void DdrExtract573Flash(string gamePath, string cardPath, string outPath)
        {
            var args = new List<string> {"ddr", "extract-573-flash"};
            AddFiles(args, gamePath, cardPath);
            AddOutPath(args, outPath);
            _app.Run(CleanArgs(args));
        }

        public void DdrDecrypt573Audio(string files, string outPath)
        {
            var args = new List<string> {"ddr", "decrypt-573-audio"};
            AddFiles(args, files);
            AddOutPath(args, outPath);
            _app.Run(CleanArgs(args));
        }
    }
}