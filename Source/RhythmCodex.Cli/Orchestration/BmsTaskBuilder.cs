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
public class BmsTaskBuilder : TaskBuilderBase<BmsTaskBuilder>
{
    private readonly IBmsStreamReader _bmsStreamReader;
    private readonly IBmsRandomResolver _bmsRandomResolver;
    private readonly IBmsDecoder _bmsDecoder;
    private readonly IChartRenderer _chartRenderer;
    private readonly IRiffPcm16SoundEncoder _riffPcm16SoundEncoder;
    private readonly IRiffStreamWriter _riffStreamWriter;
    private readonly IBmsSoundLoader _bmsSoundLoader;
    private readonly IAudioDsp _audioDsp;

    public BmsTaskBuilder(
        IFileSystem fileSystem, 
        ILogger logger,
        IBmsStreamReader bmsStreamReader,
        IBmsRandomResolver bmsRandomResolver,
        IBmsDecoder bmsDecoder,
        IChartRenderer chartRenderer,
        IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
        IRiffStreamWriter riffStreamWriter,
        IBmsSoundLoader bmsSoundLoader,
        IAudioDsp audioDsp
    ) 
        : base(fileSystem, logger)
    {
        _bmsStreamReader = bmsStreamReader;
        _bmsRandomResolver = bmsRandomResolver;
        _bmsDecoder = bmsDecoder;
        _chartRenderer = chartRenderer;
        _riffPcm16SoundEncoder = riffPcm16SoundEncoder;
        _riffStreamWriter = riffStreamWriter;
        _bmsSoundLoader = bmsSoundLoader;
        _audioDsp = audioDsp;
    }

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
                var commands = _bmsStreamReader.Read(stream);
                var resolved = _bmsRandomResolver.Resolve(commands);
                var decoded = _bmsDecoder.Decode(resolved);
                decoded.Chart.PopulateLinearOffsets();
                var sounds = _bmsSoundLoader.Load(decoded.SoundMap, accessor);
                var rendered = _chartRenderer.Render(decoded.Chart.Events, sounds, options);
                var normalized = _audioDsp.Normalize(rendered, 1.0f, true);

                using var outFile = OpenWriteSingle(task, file, i => $"{i}.render.wav");
                var riff = _riffPcm16SoundEncoder.Encode(normalized);
                _riffStreamWriter.Write(outFile, riff);
                outFile.Flush();
            });

            return true;
        });
    }
}