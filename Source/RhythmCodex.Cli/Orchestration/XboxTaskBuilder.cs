using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
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
using RhythmCodex.Xact.Streamers;

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
        private readonly IXwbStreamReader _xwbStreamReader;

        public XboxTaskBuilder(
            IFileSystem fileSystem,
            ILogger logger,
            IImaAdpcmDecoder imaAdpcmDecoder,
            IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
            IRiffStreamWriter riffStreamWriter,
            IDdsStreamReader ddsStreamReader,
            IDdsBitmapDecoder ddsBitmapDecoder,
            IPngStreamWriter pngStreamWriter,
            IXwbStreamReader xwbStreamReader)
            : base(fileSystem, logger)
        {
            _imaAdpcmDecoder = imaAdpcmDecoder;
            _riffPcm16SoundEncoder = riffPcm16SoundEncoder;
            _riffStreamWriter = riffStreamWriter;
            _ddsStreamReader = ddsStreamReader;
            _ddsBitmapDecoder = ddsBitmapDecoder;
            _pngStreamWriter = pngStreamWriter;
            _xwbStreamReader = xwbStreamReader;
        }

        public ITask CreateDecodeDds()
        {
            return Build("Decode DirectDraw Surface", task =>
            {
                var files = GetInputFiles(task);
                if (!files.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                ParallelProgress(task, files, file =>
                {
                    using (var stream = OpenRead(task, file))
                    {
                        var image = _ddsStreamReader.Read(stream, (int) stream.Length);
                        task.Message = "Decoding DDS.";
                        var bitmap = _ddsBitmapDecoder.Decode(image);
                        using (var outStream = OpenWriteSingle(task, file, i => $"{i}.png"))
                            _pngStreamWriter.Write(outStream, bitmap);
                    }
                });

                return true;
            });
        }

        public ITask CreateExtractXwb()
        {
            return Build("Extract XWB", task =>
            {
                var files = GetInputFiles(task);
                if (!files.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                ParallelProgress(task, files, file =>
                {
                    using (var stream = OpenRead(task, file))
                    {
                        var index = 0;
                        foreach (var sound in _xwbStreamReader.Read(stream))
                        {
                            var encoded = _riffPcm16SoundEncoder.Encode(sound);
                            var name = sound[StringData.Name];
                            using (var outStream = OpenWriteSingle(task, file, i => $"{name ?? $"{i}{index:0000}"}.wav"))
                            {
                                _riffStreamWriter.Write(outStream, encoded);
                                outStream.Flush();
                            }
                        }
                    }
                });

                return true;
            });
        }

        public ITask CreateDecodeXst()
        {
            return Build("Decode Xbox ADPCM", task =>
            {
                var files = GetInputFiles(task);
                if (!files.Any())
                {
                    task.Message = "No input files.";
                    return false;
                }

                ParallelProgress(task, files, file =>
                {
                    var sound = _imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                    {
                        Channels = 2,
                        ChannelSamplesPerFrame = 64,
                        Data = GetFile(task, file),
                        Rate = 44100
                    }).Single();

                    var encoded = _riffPcm16SoundEncoder.Encode(sound);
                    using (var outStream = OpenWriteSingle(task, file, i => $"{i}.wav"))
                    {
                        _riffStreamWriter.Write(outStream, encoded);
                        outStream.Flush();
                    }
                });

                return true;
            });
        }
    }
}