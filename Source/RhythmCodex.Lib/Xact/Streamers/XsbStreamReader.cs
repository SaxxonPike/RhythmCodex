using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xact.Model;
using RhythmCodex.Xact.Processors;

namespace RhythmCodex.Xact.Streamers;

[Service]
public class XsbStreamReader(
    IXsbHeaderStreamReader xsbHeaderStreamReader,
    IXsbCueNameTableStreamReader xsbCueNameTableStreamReader,
    IXsbCueStreamReader xsbCueStreamReader,
    IXsbSoundStreamReader xsbSoundStreamReader,
    IFcs16Calculator fcs16Calculator,
    ILogger logger)
    : IXsbStreamReader
{
    public XsbFile Read(Stream stream, long length)
    {
        var block = stream.TryRead(0, (int)length);
        var mem = new MemoryStream(block);
        var reader = new BinaryReader(mem);
        
        var header = xsbHeaderStreamReader.Read(mem);
        var result = new XsbFile
        {
            Header = header,
            Cues = []
        };

        var crc = fcs16Calculator.Calculate(block.AsSpan(18));
        if (crc != header.Crc)
            logger.Debug($"CRC FCS-16 does not match. Expected:{crc:X4} Found:{header.Crc:X4}");

        var waveBankNames = new string[result.Header.WaveBankCount];
        mem.Position = result.Header.WaveBankNameTableOffset;
        for (var i = 0; i < result.Header.WaveBankCount; i++)
        {
            waveBankNames[i] = reader.ReadBytes(XwbConstants.WavebankBanknameLength)
                .TakeWhile(b => b != 0x00).ToArray().GetString();
        }

        var cues = result.Cues;
        if (result.Header is { SimpleCueCount: > 0, SimpleCuesOffset: > 0 })
        {
            mem.Position = result.Header.SimpleCuesOffset;
            cues.AddRange(xsbCueStreamReader.ReadSimple(mem, result.Header.SimpleCueCount));
        }

        if (result.Header is { ComplexCueCount: > 0, ComplexCuesOffset: > 0 })
        {
            mem.Position = result.Header.ComplexCuesOffset;
            cues.AddRange(xsbCueStreamReader.ReadComplex(mem, result.Header.ComplexCueCount));
        }

        var cueNames = Array.Empty<string>();
        if (result.Header is { CueNameHashTableOffset: > 0, CueNameHashValuesOffset: > 0 })
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
        else if (result.Header is { CueNamesOffset: > 0, CueNameTableLength: > 0 })
        {
            mem.Position = result.Header.CueNamesOffset;
            cueNames = xsbCueNameTableStreamReader.Read(mem, result.Header.CueNameTableLength).ToArray();
        }

        for (var i = 0; i < cueNames.Length; i++)
        {
            var cue = cues[i];
            cue.Name = cueNames[i];
            mem.Position = cue.Offset;
            cue.Sound = xsbSoundStreamReader.Read(mem);
            cues[i] = cue;
        }

        return result;
    }
}