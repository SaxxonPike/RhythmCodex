using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RhythmCodex.Audio;
using RhythmCodex.Djmain.Model;
using RhythmCodex.Djmain.Streamers;

namespace RhythmCodex.Djmain.Converters
{
    public class DjmainSampleDecoder
    {
        private readonly IAudioStreamReader _audioStreamReader;
        private readonly IDjmainSampleDefinitionStreamReader _djmainSampleDefinitionStreamReader;
        private readonly IDjmainConfiguration _djmainConfiguration;

        public DjmainSampleDecoder(
            IAudioStreamReader audioStreamReader,
            IDjmainSampleDefinitionStreamReader djmainSampleDefinitionStreamReader,
            IDjmainConfiguration djmainConfiguration)
        {
            _audioStreamReader = audioStreamReader;
            _djmainSampleDefinitionStreamReader = djmainSampleDefinitionStreamReader;
            _djmainConfiguration = djmainConfiguration;
        }

        public IList<DjmainSample> Decode(byte[] data, int sampleOffset)
        {
            return DecodeInternal(data, sampleOffset).ToList();
        }

        private IEnumerable<DjmainSample> DecodeInternal(byte[] data, int sampleOffset)
        {
            using (var mem = new MemoryStream(data))
            {
                mem.Position = 0;

                var infos = _djmainSampleDefinitionStreamReader.Read(mem);

                var samples = infos.Select(info =>
                {
                    var id = info.Key;
                    var props = info.Value;

                });
            }
        }
    }
}
