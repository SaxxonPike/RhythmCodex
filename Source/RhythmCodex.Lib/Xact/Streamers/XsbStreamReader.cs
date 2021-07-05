using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;
using RhythmCodex.Xact.Processors;

namespace RhythmCodex.Xact.Streamers
{
    [Service]
    public class XsbStreamReader : IXsbStreamReader
    {
        private readonly IXsbHeaderStreamReader _xsbHeaderStreamReader;
        private readonly IXsbCueNameTableStreamReader _xsbCueNameTableStreamReader;
        private readonly IXsbCueStreamReader _xsbCueStreamReader;
        private readonly IXsbSoundStreamReader _xsbSoundStreamReader;
        private readonly IFcs16Calculator _fcs16Calculator;
        private readonly ILogger _logger;

        public XsbStreamReader(
            IXsbHeaderStreamReader xsbHeaderStreamReader,
            IXsbCueNameTableStreamReader xsbCueNameTableStreamReader,
            IXsbCueStreamReader xsbCueStreamReader,
            IXsbSoundStreamReader xsbSoundStreamReader,
            IFcs16Calculator fcs16Calculator,
            ILogger logger)
        {
            _xsbHeaderStreamReader = xsbHeaderStreamReader;
            _xsbCueNameTableStreamReader = xsbCueNameTableStreamReader;
            _xsbCueStreamReader = xsbCueStreamReader;
            _xsbSoundStreamReader = xsbSoundStreamReader;
            _fcs16Calculator = fcs16Calculator;
            _logger = logger;
        }

        public XsbFile Read(Stream stream, long length)
        {
            var block = stream.TryRead(0, length);
            var mem = new MemoryStream(block);
            var reader = new BinaryReader(mem);
            var result = new XsbFile();

            var header = _xsbHeaderStreamReader.Read(mem);
            result.Header = header;

            var crc = _fcs16Calculator.Calculate(block.AsSpan(18));
            if (crc != header.Crc)
                _logger.Debug($"CRC FCS-16 does not match. Expected:{crc:X4} Found:{header.Crc:X4}");

            var waveBankNames = new string[result.Header.WaveBankCount];
            mem.Position = result.Header.WaveBankNameTableOffset;
            for (var i = 0; i < result.Header.WaveBankCount; i++)
            {
                waveBankNames[i] = reader.ReadBytes(XwbConstants.WavebankBanknameLength)
                    .TakeWhile(b => b != 0x00).ToArray().GetString();
            }

            var cues = new List<XsbCue>();
            if (result.Header.SimpleCueCount > 0 && result.Header.SimpleCuesOffset > 0)
            {
                mem.Position = result.Header.SimpleCuesOffset;
                cues.AddRange(_xsbCueStreamReader.ReadSimple(mem, result.Header.SimpleCueCount));
            }

            if (result.Header.ComplexCueCount > 0 && result.Header.ComplexCuesOffset > 0)
            {
                mem.Position = result.Header.ComplexCuesOffset;
                cues.AddRange(_xsbCueStreamReader.ReadComplex(mem, result.Header.ComplexCueCount));
            }

            var cueNames = Array.Empty<string>();
            if (result.Header.CueNameHashTableOffset > 0 && result.Header.CueNameHashValuesOffset > 0)
            {
                var cueHashIds = new short[result.Header.TotalCueCount];
                mem.Position = result.Header.CueNameHashTableOffset;
                for (var i = 0; i < result.Header.TotalCueCount; i++)
                    cueHashIds[i] = reader.ReadInt16();

                var cueHashNameOffsets = new int[cues.Count];
                var cueHashValue = new short[cues.Count];
                mem.Position = result.Header.CueNameHashValuesOffset;
                for (var i = 0; i < cues.Count; i++)
                {
                    cueHashNameOffsets[i] = reader.ReadInt32();
                    cueHashValue[i] = reader.ReadInt16();
                }

                cueNames = new string[cues.Count];
                for (var i = 0; i < cues.Count; i++)
                {
                    var hashName = new List<byte>();
                    mem.Position = cueHashNameOffsets[i];
                    while (true)
                    {
                        var b = reader.ReadByte();
                        if (b == 0x00)
                            break;
                        hashName.Add(b);
                    }

                    cueNames[i] = hashName.ToArray().GetString();
                }
            }
            else if (result.Header.CueNamesOffset > 0 && result.Header.CueNameTableLength > 0)
            {
                mem.Position = result.Header.CueNamesOffset;
                cueNames = _xsbCueNameTableStreamReader.Read(mem, result.Header.CueNameTableLength).ToArray();
            }

            for (var i = 0; i < cueNames.Length; i++)
            {
                var cue = cues[i];
                cue.Name = cueNames[i];
                mem.Position = cue.Offset;
                cue.Sound = _xsbSoundStreamReader.Read(mem);
                cues[i] = cue;
            }

            result.Cues = cues.ToArray();

            return result;
        }
    }
}