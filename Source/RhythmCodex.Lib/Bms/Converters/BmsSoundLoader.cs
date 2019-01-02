using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Charting;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.ThirdParty;

namespace RhythmCodex.Bms.Converters
{
    [Service]
    public class BmsSoundLoader : IBmsSoundLoader
    {
        private static readonly string[] ExtensionPriority =
        {
            "wav",
            "flac",
            "ogg",
            "mp3"
        };

        private readonly IRiffStreamReader _riffStreamReader;
        private readonly IMp3Decoder _mp3Decoder;
        private readonly IOggDecoder _oggDecoder;
        private readonly IFlacDecoder _flacDecoder;

        public BmsSoundLoader(
            IRiffStreamReader riffStreamReader,
            IMp3Decoder mp3Decoder,
            IOggDecoder oggDecoder,
            IFlacDecoder flacDecoder)
        {
            _riffStreamReader = riffStreamReader;
            _mp3Decoder = mp3Decoder;
            _oggDecoder = oggDecoder;
            _flacDecoder = flacDecoder;
        }

        public IList<ISound> Load(IDictionary<int, string> map, IFileAccessor accessor)
        {
            return LoadInternal(map, accessor).ToList();
        }

        private IEnumerable<ISound> LoadInternal(IDictionary<int, string> map, IFileAccessor accessor)
        {
            foreach (var kv in map)
            {
                var actualFileName = GetActualFileName(Path.GetFileNameWithoutExtension(kv.Value), accessor);
                if (actualFileName == null)
                    continue;

                using (var stream = accessor.OpenRead(actualFileName))
                {
                }
            }

            throw new NotImplementedException();
        }

        private string GetActualFileName(string name, IFileAccessor accessor)
        {
            if (accessor.FileExists(name))
                return name;

            var baseName = Path.GetFileNameWithoutExtension(name);
            foreach (var extension in ExtensionPriority)
            {
                var desiredName = $"{baseName}.{extension}";
                if (accessor.FileExists(desiredName))
                    return desiredName;
            }

            return null;
        }
    }
}