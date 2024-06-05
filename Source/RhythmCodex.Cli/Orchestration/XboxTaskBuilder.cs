using System.IO;
using System.Linq;
using ClientCommon;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.ImaAdpcm.Converters;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Xact.Converters;
using RhythmCodex.Xact.Streamers;
using RhythmCodex.Xbox.Streamers;

namespace RhythmCodex.Cli.Orchestration;

[Service(singleInstance: false)]
public class XboxTaskBuilder(
    IFileSystem fileSystem,
    ILogger logger,
    IImaAdpcmDecoder imaAdpcmDecoder,
    IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
    IRiffStreamWriter riffStreamWriter,
    IXwbStreamReader xwbStreamReader,
    IXboxIsoStreamReader xboxIsoStreamReader,
    IXboxSngStreamReader xboxSngStreamReader,
    IXboxHbnStreamReader xboxHbnStreamReader,
    IXwbDecoder xwbDecoder)
    : TaskBuilderBase<XboxTaskBuilder>(fileSystem, logger)
{
    public ITask CreateExtractHbn()
    {
        return Build("Extract HBN", task =>
        {
            var files = GetInputFiles(task);

            if (!files.Any())
            {
                task.Message = "No input files.";
                return false;
            }

            ParallelProgress(task, files, file =>
            {
                using var stream = OpenRead(task, file);
                var binFileName = $"{Path.GetFileNameWithoutExtension(file.Name)}.bin";
                        
                var binFile = Path.Combine(
                    Path.GetDirectoryName(file.Name),
                    binFileName);

                foreach (var entry in xboxHbnStreamReader.Read(stream, OpenRead(task, GetInputFileDirect(binFile))))
                {
                    using var outStream =
                        OpenWriteMulti(task, file, _ => entry.Name);
                    var writer = new BinaryWriter(outStream);
                    writer.Write(entry.Data);
                    outStream.Flush();
                }
            });

            return true;
        });
    }

    public ITask CreateExtractXiso()
    {
        return Build("Extract XISO", task =>
        {
            var files = GetInputFiles(task);
            if (!files.Any())
            {
                task.Message = "No input files.";
                return false;
            }

            ParallelProgress(task, files, file =>
            {
                using var stream = OpenRead(task, file);
                var index = 0;
                foreach (var entry in xboxIsoStreamReader.Read(stream, stream.Length))
                {
                    using (var outStream =
                           OpenWriteMulti(task, file, _ => entry.FileName))
                    {
                        outStream.Write(xboxIsoStreamReader.Extract(stream, entry).Span);
                        outStream.Flush();
                    }

                    index++;
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
                using var stream = OpenRead(task, file);
                var index = 0;
                foreach (var sound in xwbStreamReader.Read(stream))
                {
                    var decoded = xwbDecoder.Decode(sound);
                    var encoded = riffPcm16SoundEncoder.Encode(decoded);
                    var name = decoded[StringData.Name];
                    using (var outStream =
                           OpenWriteSingle(task, file, i => $"{name ?? $"{i}{index:0000}"}.wav"))
                    {
                        riffStreamWriter.Write(outStream, encoded);
                        outStream.Flush();
                    }

                    index++;
                }
            });

            return true;
        });
    }

    public ITask CreateExtractSng()
    {
        return Build("Extract SNG", task =>
        {
            var files = GetInputFiles(task);
            if (!files.Any())
            {
                task.Message = "No input files.";
                return false;
            }

            ParallelProgress(task, files, file =>
            {
                using var input = OpenRead(task, file);
                foreach (var entry in xboxSngStreamReader.Read(input))
                {
                    if (entry.Song != null)
                    {
                        var sound = imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                        {
                            Channels = 2,
                            ChannelSamplesPerFrame = 64,
                            Data = entry.Song,
                            Rate = 44100
                        });

                        var encoded = riffPcm16SoundEncoder.Encode(sound);
                        using var outStream = OpenWriteMulti(task, file, _ => $"{entry.Name}.wav");
                        riffStreamWriter.Write(outStream, encoded);
                        outStream.Flush();
                    }

                    if (entry.Preview != null)
                    {
                        var sound = imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                        {
                            Channels = 2,
                            ChannelSamplesPerFrame = 64,
                            Data = entry.Preview,
                            Rate = 44100
                        });

                        var encoded = riffPcm16SoundEncoder.Encode(sound);
                        using var outStream = OpenWriteMulti(task, file, _ => $"{entry.Name}-preview.wav");
                        riffStreamWriter.Write(outStream, encoded);
                        outStream.Flush();
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
                var sound = imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                {
                    Channels = 2,
                    ChannelSamplesPerFrame = 64,
                    Data = GetFile(task, file),
                    Rate = 44100
                });

                var encoded = riffPcm16SoundEncoder.Encode(sound);
                using var outStream = OpenWriteSingle(task, file, i => $"{i}.wav");
                riffStreamWriter.Write(outStream, encoded);
                outStream.Flush();
            });

            return true;
        });
    }
}