using System.IO;
using System.Linq;
using ClientCommon;
using RhythmCodex.Bms.Converters;
using RhythmCodex.Bms.Streamers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Sounds.Converters;
using RhythmCodex.Wav.Converters;
using RhythmCodex.Wav.Models;

namespace RhythmCodex.Cli.Orchestration;

[Service(singleInstance: false)]
public class BmsTaskBuilder(
    IFileSystem fileSystem,
    ILogger logger,
    IBmsStreamReader bmsStreamReader,
    IBmsRandomResolver bmsRandomResolver,
    IBmsDecoder bmsDecoder,
    IChartRenderer chartRenderer,
    IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
    IRiffStreamWriter riffStreamWriter,
    IBmsSoundLoader bmsSoundLoader,
    IAudioDsp audioDsp)
    : TaskBuilderBase<BmsTaskBuilder>(fileSystem, logger)
{
    public ITask CreateRenderBms()
    {
        return Build("BMS to WAV", task =>
        {
            var files = GetInputFiles(task);
            if (!files.Any())
            {
                task.Message = "No input files.";
                return false;
            }

            var options = new ChartRendererOptions
            {
                UseSourceDataForSamples = true
            };

            ParallelProgress(task, files, file =>
            {
                using var stream = OpenRead(task, file);
                var accessor = new FileAccessor(Path.GetDirectoryName(file.Name));
                var commands = bmsStreamReader.Read(stream);
                var resolved = bmsRandomResolver.Resolve(commands);
                var decoded = bmsDecoder.Decode(resolved);
                decoded.Chart.PopulateLinearOffsets();
                var sounds = bmsSoundLoader.Load(decoded.SoundMap, accessor);
                var rendered = chartRenderer.Render(decoded.Chart.Events, sounds, options);
                var normalized = audioDsp.Normalize(rendered, 1.0f, true);

                using var outFile = OpenWriteSingle(task, file, i => $"{i}.render.wav");
                var riff = riffPcm16SoundEncoder.Encode(normalized);
                riffStreamWriter.Write(outFile, riff);
                outFile.Flush();
            });

            return true;
        });
    }
}