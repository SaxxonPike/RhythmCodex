using System.IO;
using System.Linq;
using ClientCommon;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Xact.Converters;
using RhythmCodex.Xact.Streamers;

namespace XwbExtract;

public class App : IApp
{
    private readonly IFileSystem _fileSystem;
    private readonly IXwbStreamReader _xwbStreamReader;
    private readonly IXwbDecoder _xwbDecoder;
    private readonly IRiffPcm16SoundEncoder _riffPcm16SoundEncoder;
    private readonly IRiffStreamWriter _riffStreamWriter;
    private readonly IArgResolver _argResolver;

    public App(
        IFileSystem fileSystem,
        IXwbStreamReader xwbStreamReader,
        IXwbDecoder xwbDecoder,
        IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
        IRiffStreamWriter riffStreamWriter,
        IArgResolver argResolver
    )
    {
        _fileSystem = fileSystem;
        _xwbStreamReader = xwbStreamReader;
        _xwbDecoder = xwbDecoder;
        _riffPcm16SoundEncoder = riffPcm16SoundEncoder;
        _riffStreamWriter = riffStreamWriter;
        _argResolver = argResolver;
    }

    public void Run(TextWriter log, Args args)
    {
        var outFolder = _argResolver.GetOutputDirectory(args);
        var files = _argResolver.GetInputFiles(args).ToArray();

        Iteration.ForEach(files, file =>
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            using var inStream = _fileSystem.OpenRead(file);
            var xwbSounds = _xwbStreamReader.Read(inStream).ToArray();
            Iteration.ForEach(xwbSounds, xwbSound =>
            {
                var decoded = _xwbDecoder.Decode(xwbSound);
                var riff = _riffPcm16SoundEncoder.Encode(decoded);
                var outPath = Path.Combine(outFolder, fileName,
                    _fileSystem.GetSafeFileName(xwbSound.Name) + ".wav");
                using var outStream = _fileSystem.OpenWrite(outPath);
                _riffStreamWriter.Write(outStream, riff);
                outStream.Flush();
            });
        });
    }

    public void Usage(TextWriter log)
    {
        log.WriteLine(@"XwbExtract [files] [-o <path>] [+r]");
    }
}