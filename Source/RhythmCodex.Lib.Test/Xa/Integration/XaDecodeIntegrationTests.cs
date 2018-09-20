using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.Attributes;
using RhythmCodex.Infrastructure.Converters;
using RhythmCodex.Iso.Converters;
using RhythmCodex.Iso.Streamers;
using RhythmCodex.Riff.Converters;
using RhythmCodex.Riff.Streamers;
using RhythmCodex.Vag.Converters;
using RhythmCodex.Vag.Streamers;
using RhythmCodex.Xa.Converters;
using RhythmCodex.Xa.Models;

namespace RhythmCodex.Xa.Integration
{
    [TestFixture]
    public class XaDecodeIntegrationTests : BaseIntegrationFixture
    {
//        [Test]
//        [Explicit]
//        public void Test_Xa()
//        {
//            var data = GetArchiveResource($"Xa.xa.zip")
//                .First()
//                .Value;
////            var data = File.ReadAllBytes($@"Z:\MCHDATA.PAK");
//
//            var decoder = Resolve<IXaDecoder>();
//            var encoder = Resolve<IRiffPcm16SoundEncoder>();
//            var writer = Resolve<IRiffStreamWriter>();
//            var xa = new XaChunk
//            {
//                Data = data,
//                Channels = 4,
//            };
//
//            var decoded = decoder.Decode(xa);
//            var index = 0;
//
//            foreach (var sound in decoded)
//            {
//                sound[NumericData.Rate] = 37800;
//                var encoded = encoder.Encode(sound);
//                using (var outStream = new MemoryStream())
//                {
//                    writer.Write(outStream, encoded);
//                    outStream.Flush();
//                    File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"xa-{index}.wav"), outStream.ToArray());
//                    index++;
//                }
//            }
//        }
        
        [Test]
        [Explicit]
        public void Test_Xa_Via_Bin()
        {
            var data = File.ReadAllBytes(@"\\tamarat\Games\PS1\Beatmania 2nd Mix CD B Append.img");

            var isoReader = Resolve<IIsoSectorStreamReader>();
            var isoInfoDecoder = Resolve<IIsoSectorInfoDecoder>();
            var channelGroups = isoReader
                .Read(new MemoryStream(data), data.Length)
                .Select(s => isoInfoDecoder.Decode(s))
                .Where(s => s.IsAudio ?? false)
                .OrderBy(s => (s.Minutes << 16) | (s.Seconds << 8) | s.Frames)
                .GroupBy(s => s.Channel)
                .ToList();
            
            var decoder = Resolve<IXaDecoder>();
            var encoder = Resolve<IRiffPcm16SoundEncoder>();
            var writer = Resolve<IRiffStreamWriter>();
            var slicer = Resolve<ISlicer>();
            var index = 0;

            foreach (var channelGroup in channelGroups)
            {
                var rawData = channelGroup.SelectMany(c => slicer.Slice(c.Data, c.UserDataOffset, c.UserDataLength)).ToArray();
                var xa = new XaChunk
                {
                    Channels = channelGroup.First().AudioChannels.Value,
                    Data = rawData
                };
                
                var decoded = decoder.Decode(xa);

                foreach (var sound in decoded)
                {
                    sound[NumericData.Rate] = channelGroup.First().AudioRate;
                    var encoded = encoder.Encode(sound);
                    using (var outStream = new MemoryStream())
                    {
                        writer.Write(outStream, encoded);
                        outStream.Flush();
                        File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"xa-{index}.wav"), outStream.ToArray());
                        index++;
                    }
                }
            }
        }
        
    }
}