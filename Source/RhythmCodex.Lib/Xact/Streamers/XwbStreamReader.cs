using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Attributes;
using RhythmCodex.Extensions;
using RhythmCodex.ImaAdpcm.Converters;
using RhythmCodex.ImaAdpcm.Models;
using RhythmCodex.Infrastructure;
using RhythmCodex.Infrastructure.Models;
using RhythmCodex.Wav.Converters;
using RhythmCodex.Wav.Models;
using RhythmCodex.Xact.Model;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XwbStreamReader : IXwbStreamReader
    {
        private readonly IPcmDecoder _pcmDecoder;
        private readonly IImaAdpcmDecoder _imaAdpcmDecoder;
        private readonly IMicrosoftAdpcmDecoder _microsoftAdpcmDecoder;

        public XwbStreamReader(
            IPcmDecoder pcmDecoder,
            IImaAdpcmDecoder imaAdpcmDecoder,
            IMicrosoftAdpcmDecoder microsoftAdpcmDecoder)
        {
            _pcmDecoder = pcmDecoder;
            _imaAdpcmDecoder = imaAdpcmDecoder;
            _microsoftAdpcmDecoder = microsoftAdpcmDecoder;
        }

        public IList<ISound> Read(Stream source)
        {
            var result = new List<ISound>();
            using (var reader = new BinaryReader(source))
            {
                var sampleCount = 0;
                var header = new WaveBankHeader();
                var bank = new WaveBankData();
                WaveBankEntry[] entries = { };
                string[] names = { };
                var dataChunk = new MemoryStream();

                header = WaveBankHeader.Read(source);

                for (var i = 0; i < (int) WaveBankSegIdx.Count; i++)
                {
                    var region = header.Segments[i];
                    if (region.Length > 0)
                    {
                        source.Position = region.Offset;
                        var mem = new MemoryStream(reader.ReadBytes(region.Length));
                        var memReader = new BinaryReader(mem);
                        switch (i)
                        {
                            case (int) WaveBankSegIdx.BankData:
                                bank = WaveBankData.Read(mem);
                                sampleCount = bank.EntryCount;
                                entries = new WaveBankEntry[sampleCount];
                                names = new string[sampleCount];
                                mem.Dispose();
                                break;
                            case (int) WaveBankSegIdx.EntryMetaData:
                                for (var j = 0; j < sampleCount; j++)
                                    entries[j] = WaveBankEntry.Read(mem);
                                mem.Dispose();
                                break;
                            case (int) WaveBankSegIdx.EntryNames:
                                for (var j = 0; j < sampleCount; j++)
                                    names[j] = new string(memReader.ReadChars(XactConstants.WavebankEntrynameLength)
                                        .TakeWhile(c => c != 0).ToArray());
                                mem.Dispose();
                                break;
                            case (int) WaveBankSegIdx.EntryWaveData:
                                dataChunk = mem;
                                break;
                            case (int) WaveBankSegIdx.SeekTables:
                                mem.Dispose();
                                break;
                            default:
                                mem.Dispose();
                                break;
                        }
                    }
                }

                if (sampleCount > 0)
                {
                    var sounds = new List<ISound>();
                    for (var i = 0; i < sampleCount; i++)
                    {
                        var entry = entries[i];
                        dataChunk.Position = entry.PlayRegion.Offset;
                        var sound = DecodeSound(entry.Format, dataChunk, entry.PlayRegion.Length);

                        if (sound != null)
                            sounds.Add(sound);
                    }
                }
            }

            return result;
        }

        private ISound DecodeSound(WaveBankMiniWaveFormat format, Stream stream, int length)
        {
            var result = new Sound();

            switch (format.FormatTag)
            {
                case XactConstants.WavebankminiformatTagPcm:
                {
                    var reader = new BinaryReader(stream);
                    var data = reader.ReadBytes(length);
                    float[] decoded;
                    switch (format.BitsPerSample)
                    {
                        case 8:
                            decoded = _pcmDecoder.Decode8Bit(data);
                            break;
                        case 16:
                            decoded = _pcmDecoder.Decode16Bit(data);
                            break;
                        case 24:
                            decoded = _pcmDecoder.Decode24Bit(data);
                            break;
                        case 32:
                            decoded = _pcmDecoder.Decode32Bit(data);
                            break;
                        default:
                            return null;
                    }

                    foreach (var channel in decoded.Deinterleave(1, format.Channels))
                    {
                        result.Samples.Add(new Sample
                        {
                            [NumericData.Rate] = format.SampleRate,
                            Data = channel
                        });
                    }

                    return result;
                }
                case XactConstants.WavebankminiformatTagXma:
                {
                    var reader = new BinaryReader(stream);
                    return _imaAdpcmDecoder.Decode(new ImaAdpcmChunk
                    {
                        Channels = format.Channels,
                        ChannelSamplesPerFrame = format.AdpcmSamplesPerBlock,
                        Data = reader.ReadBytes(length),
                        Rate = format.SampleRate
                    }).SingleOrDefault();
                }
                case XactConstants.WavebankminiformatTagAdpcm:
                {
                    var reader = new BinaryReader(stream);
                    return _microsoftAdpcmDecoder.Decode(
                        reader.ReadBytes(length),
                        format,
                        new MsAdpcmFormat
                        {
                            Coefficients = new int[0],
                            SamplesPerBlock = format.AdpcmSamplesPerBlock
                        });
                }
                default:
                {
                    return null;
                }
            }
        }
    }
}