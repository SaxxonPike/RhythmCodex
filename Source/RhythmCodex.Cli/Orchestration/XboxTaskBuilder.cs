using System.IO;
using System.Linq;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Dds.Converters;
using RhythmCodex.Dds.Streamers;
using RhythmCodex.Gdi.Streamers;
using RhythmCodex.ImaAdpcm.Converters;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.Cli.Orchestration
{
    [InstancePerDependency]
    public class XboxTaskBuilder : TaskBuilderBase<XboxTaskBuilder>
    {
        private readonly IImaAdpcmDecoder _imaAdpcmDecoder;
        private readonly IRiffPcm16SoundEncoder _riffPcm16SoundEncoder;
        private readonly IRiffStreamWriter _riffStreamWriter;
        private readonly IDdsStreamReader _ddsStreamReader;
        private readonly IDdsBitmapDecoder _ddsBitmapDecoder;
        private readonly IPngStreamWriter _pngStreamWriter;

        public XboxTaskBuilder(
            IFileSystem fileSystem,
            ILogger logger,
            IImaAdpcmDecoder imaAdpcmDecoder,
            IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
            IRiffStreamWriter riffStreamWriter,
            IDdsStreamReader ddsStreamReader,
            IDdsBitmapDecoder ddsBitmapDecoder,
            IPngStreamWriter pngStreamWriter)
            : base(fileSystem, logger)
        {
            _imaAdpcmDecoder = imaAdpcmDecoder;
            _riffPcm16SoundEncoder = riffPcm16SoundEncoder;
            _riffStreamWriter = riffStreamWriter;
            _ddsStreamReader = ddsStreamReader;
            _ddsBitmapDecoder = ddsBitmapDecoder;
            _pngStreamWriter = pngStreamWriter;
        }

        public ITask CreateDecodeDds()
        {
            return Build("Decode DirectDraw Surface", task =>
            {
                var inputFiles = GetInputFiles(task);
                if (!inputFiles.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                foreach (var inputFile in inputFiles)
                {
                    using (var stream = OpenRead(task, inputFile))
                    {
                        var image = _ddsStreamReader.Read(stream, (int) stream.Length);
                        task.Message = "Decoding DDS.";
                        var bitmap = _ddsBitmapDecoder.Decode(image);
                        using (var outStream = OpenWriteSingle(task, inputFile, i => $"{i}.png"))
                            _pngStreamWriter.Write(outStream, bitmap);
                    }
                }

                return true;
            });
        }

        public ITask CreateDecodeAdpcm()
        {
            return Build("Decode Xbox ADPCM", task =>
            {
                var inputFiles = GetInputFiles(task);
                if (!inputFiles.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                foreach (var inputFile in inputFiles)
                {
                    var sound = _imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                    {
                        Channels = 2,
                        ChannelSamplesPerFrame = 64,
                        Data = GetFile(task, inputFile),
                        Rate = 44100
                    }).Single();

                    var encoded = _riffPcm16SoundEncoder.Encode(sound);
                    using (var outStream = OpenWriteSingle(task, inputFile, i => $"{i}.wav"))
                    {
                        _riffStreamWriter.Write(outStream, encoded);
                        outStream.Flush();
                    }
                }

                return true;
            });
        }
    }
}