using System;
using System.IO;
using System.Linq;
using CSCore.Codecs.FLAC;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.IoC;
using RhythmCodex.Meta.Models;
using RhythmCodex.Sounds.Models;

namespace RhythmCodex.ThirdParty
{
    [Service]
    public class FlacDecoder : IFlacDecoder
    {
        public ISound Decode(Stream stream)
        {
            using (var inputStream = new FlacFile(stream))
            {
                var samples = StreamExtensions.ReadAllBytes(inputStream.Read)
                    .Deinterleave(2, inputStream.WaveFormat.Channels)
                    .Select(bytes => new Sample
                    {
                        Data = bytes.AsArray().Fuse().Select(s => s / 32768f).AsArray()
                    })
                    .Cast<ISample>()
                    .ToList();
                
                return new Sound
                {
                    Samples = samples,
                    [NumericData.Rate] = inputStream.WaveFormat.SampleRate
                };
            }
        }

        public Memory<byte> DecodeFrame(Stream stream, int blockSize)
        {
            var frame = FlacFrame.FromStream(stream, new FlacMetadataStreamInfo
            {
                SampleRate = 44100,
                Channels = 2,
                BitsPerSample = 16
            });
            Memory<byte> buffer = null;
            frame.NextFrame();
            frame.GetBuffer(ref buffer);
            return buffer;
        }
    }
}