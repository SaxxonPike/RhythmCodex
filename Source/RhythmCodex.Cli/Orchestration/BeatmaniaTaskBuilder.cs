using System.IO;
using System.Linq;
using RhythmCodex.BeatmaniaPc.Streamers;
using RhythmCodex.Cli.Helpers;
using RhythmCodex.Cli.Orchestration.Infrastructure;
using RhythmCodex.Dsp;
using RhythmCodex.Infrastructure;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;

namespace RhythmCodex.Cli.Orchestration
{
    [InstancePerDependency]
    public class BeatmaniaTaskBuilder : TaskBuilderBase<BeatmaniaTaskBuilder>
    {
        private readonly IBeatmaniaPcAudioStreamer _beatmaniaPcAudioStreamer;
        private readonly IRiffPcm16SoundEncoder _riffPcm16SoundEncoder;
        private readonly IRiffStreamWriter _riffStreamWriter;
        private readonly IAudioDsp _audioDsp;

        public BeatmaniaTaskBuilder(
            IFileSystem fileSystem,
            ILogger logger,
            IBeatmaniaPcAudioStreamer beatmaniaPcAudioStreamer,
            IRiffPcm16SoundEncoder riffPcm16SoundEncoder,
            IRiffStreamWriter riffStreamWriter,
            IAudioDsp audioDsp
            )
            : base(fileSystem, logger)
        {
            _beatmaniaPcAudioStreamer = beatmaniaPcAudioStreamer;
            _riffPcm16SoundEncoder = riffPcm16SoundEncoder;
            _riffStreamWriter = riffStreamWriter;
            _audioDsp = audioDsp;
        }


        public ITask CreateExtract2dx()
        {
            return Build("Extract 2DX", task =>
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
                        var decrypted = _beatmaniaPcAudioStreamer.Decrypt(stream, stream.Length);
                        using (var decryptedStream = new MemoryStream(decrypted))
                        {
                            var sounds = _beatmaniaPcAudioStreamer.Read(decryptedStream, stream.Length);
                            var index = 1;
                            foreach (var sound in sounds)
                            {
                                var outSound = _audioDsp.ApplyEffects(sound);
                                using (var outStream =
                                    OpenWriteMulti(task, file, i => $"{Alphabet.EncodeAlphanumeric(index, 4)}.wav"))
                                {
                                    var encoded = _riffPcm16SoundEncoder.Encode(outSound);
                                    _riffStreamWriter.Write(outStream, encoded);
                                }

                                index++;
                            }
                        }
                    }
                });

                return true;
            });
        }
    }
}