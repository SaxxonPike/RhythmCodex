using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using RhythmCodex.FileSystems.Cd.Streamers;
using RhythmCodex.FileSystems.Iso.Converters;
using RhythmCodex.Metadatas.Models;
using RhythmCodex.Sounds.Riff.Converters;
using RhythmCodex.Sounds.Riff.Streamers;
using RhythmCodex.Sounds.Xa.Converters;
using RhythmCodex.Sounds.Xa.Heuristics;

namespace RhythmCodex.Sounds.Xa.Integration;

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
        var data = File.ReadAllBytes(@"\\tamarat\Games\PS1\Beatmania Gotta Mix 2 Append.img");

        var isoReader = Resolve<ICdSectorStreamReader>();
        var isoInfoDecoder = Resolve<IIsoSectorInfoDecoder>();
        var decoder = Resolve<IXaDecoder>();
        var encoder = Resolve<IRiffPcm16SoundEncoder>();
        var writer = Resolve<IRiffStreamWriter>();
        var streamFinder = Resolve<IXaIsoStreamFinder>();
            
        var streams = streamFinder.Find(isoReader
            .Read(new MemoryStream(data), data.Length, true)
            .Select(s => isoInfoDecoder.Decode(s)));

        var index = 0;
            
        foreach (var xa in streams)
        {
            var decoded = decoder.Decode(xa);

            foreach (var sound in decoded)
            {
                sound![NumericData.Rate] = xa.Rate;
                var encoded = encoder.Encode(sound);
                var outfolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "xa");
                if (!Directory.Exists(outfolder))
                    Directory.CreateDirectory(outfolder);

                using var outStream = new MemoryStream();
                writer.Write(outStream, encoded);
                outStream.Flush();
                File.WriteAllBytes(Path.Combine(outfolder, $"{index:000}.wav"), outStream.ToArray());
                index++;
            }
        }
    }
        
}