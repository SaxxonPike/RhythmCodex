using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RhythmCodex.Cli;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Gui
{
    [Service]
    public class GuiTasks : IGuiTasks
    {
        private readonly IApp _app;
        private readonly ILogger _logger;

        public GuiTasks(IApp app, ILogger logger)
        {
            _app = app;
            _logger = logger;
        }

        private void Run(IEnumerable<string> args)
        {
            var argsCopy = args.ToList();
            
            // okay so this is KIND of a hack- because of the limitations in the CLI, it's all or nothing
            if (argsCopy.Any(a => a.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase)))
                argsCopy.Add("+zip");

            var task = new Task(() =>
            {
                try
                {
                    _app.Run(CleanArgs(argsCopy));
                }
                catch
                {
                    throw;
                }
            });

            task.ContinueWith(t =>
            {
                _logger.Info("---end---");
                if (!t.IsFaulted)
                    return;

                _logger.Warning("The task failed.");
                _logger.Warning(t.Exception?.ToString() ?? "(no exception)");
            });
            
            task.Start();
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
            Run(args);
        }

        public void DdrDecrypt573Audio(string files, string outPath, bool decodeNames)
        {
            var args = new List<string> {"ddr", "decrypt-573-audio"};
            AddFiles(args, files);
            AddOutPath(args, outPath);
            if (decodeNames)
                args.Add("+name");
            Run(args);
        }

        public void SsqDecode(string files, string outPath, double offset)
        {
            var args = new List<string> {"ssq", "decode"};
            AddFiles(args, files);
            AddOutPath(args, outPath);
            if (offset != 0)
            {
                args.Add("-offset");
                args.Add($"{offset}");
            }
            Run(args);
        }

        public void BeatmaniaDecodeDjmainHdd(string files, string outPath, bool skipAudio, bool skipCharts, bool rawCharts)
        {
            var args = new List<string> {"bm", "decode-djmain-hdd"};
            AddFiles(args, files);
            AddOutPath(args, outPath);
            if (skipAudio)
                args.Add("+noaudio");
            if (skipCharts)
                args.Add("+nocharts");
            if (rawCharts)
                args.Add("+raw");
            Run(args);
        }

        public void BmsRender(string files, string outPath)
        {
            var args = new List<string> {"bms", "render"};
            AddFiles(args, files);
            AddOutPath(args, outPath);
            Run(args);
        }

        public void BeatmaniaRenderDjmainGst(string files, string outPath)
        {
            var args = new List<string> {"bm", "render-djmain-gst"};
            AddFiles(args, files);
            AddOutPath(args, outPath);
            Run(args);
        }
    }
}