using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Models;
using RhythmCodex.ThirdParty;
using RhythmCodex.Wav.Converters;

namespace RhythmCodex.Bms.Converters
{
    [Service]
    public class BmsSoundLoader : IBmsSoundLoader
    {
        private readonly IWavDecoder _wavDecoder;
        private readonly IMp3Decoder _mp3Decoder;
        private readonly IOggDecoder _oggDecoder;
        private readonly IFlacDecoder _flacDecoder;

        private readonly Dictionary<string, Func<Stream, ISound>> Extensions;

        public BmsSoundLoader(
            IWavDecoder wavDecoder,
            IMp3Decoder mp3Decoder,
            IOggDecoder oggDecoder,
            IFlacDecoder flacDecoder)
        {
            _wavDecoder = wavDecoder;
            _mp3Decoder = mp3Decoder;
            _oggDecoder = oggDecoder;
            _flacDecoder = flacDecoder;

            Extensions = new Dictionary<string, Func<Stream, ISound>>
            {
                {"wav", s => _wavDecoder.Decode(s)},
                {"flac", s => _flacDecoder.Decode(s)},
                {"ogg", s => _oggDecoder.Decode(s)},
                {"mp3", s => _mp3Decoder.Decode(s)}
            };
        }

        public IList<ISound> Load(IDictionary<int, string> map, IFileAccessor accessor)
        {
            return LoadInternal(map, accessor).ToList();
        }

        private IEnumerable<ISound> LoadInternal(IDictionary<int, string> map, IFileAccessor accessor)
        {
            foreach (var kv in map)
            {
                var decoder = GetDecoder(Path.GetFileNameWithoutExtension(kv.Value), accessor);
                if (decoder.Decoder == null)
                    continue;

                using (var stream = accessor.OpenRead(decoder.Filename))
                    yield return decoder.Decoder(stream);
            }
        }

        private (string Filename, Func<Stream, ISound> Decoder) GetDecoder(string name, IFileAccessor accessor)
        {
            var file = accessor.GetFileNameByExtension(name, Extensions.Keys);
            if (file == null)
                throw new RhythmCodexException($"Unable to find a decoder for {name}.");
            return (file.Filename, Extensions[file.Extension]);
        }
    }
}