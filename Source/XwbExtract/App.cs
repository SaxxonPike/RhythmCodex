using System.IO;
using System.Linq;
using ClientCommon;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Xact.Converters;
using RhythmCodex.Xact.Streamers;

namespace XwbExtract;

public class App(
    IFileSystem fileSystem,
    IXwbStreamReader xwbStreamReader,
    IXwbDecoder xwbDecoder,
    IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
    IRiffStreamWriter riffStreamWriter,
    IArgResolver argResolver)
    : IApp
{
    public void Run(TextWriter log, Args args)
    {
        var outFolder = argResolver.GetOutputDirectory(args);
        var files = argResolver.GetInputFiles(args).ToArray();

        Iteration.ForEach(files, file =>
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            using var inStream = fileSystem.OpenRead(file);
            var xwbSounds = xwbStreamReader.Read(inStream).ToArray();
            Iteration.ForEach(xwbSounds, xwbSound =>
            {
                var decoded = xwbDecoder.Decode(xwbSound);
                var riff = riffPcm16SoundEncoder.Encode(decoded);
                var outPath = Path.Combine(outFolder, fileName,
                    fileSystem.GetSafeFileName(xwbSound.Name) + ".wav");
                using var outStream = fileSystem.OpenWrite(outPath);
                riffStreamWriter.Write(outStream, riff);
                outStream.Flush();
            });
        });
    }

    public void Usage(TextWriter log)
    {
        log.WriteLine("XwbExtract [files] [-o <path>] [+r]");
    }
}