using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Models;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Beatmania.Streamers;

[Service]
public class BeatmaniaPs2OldKeysoundStreamReader : IBeatmaniaPs2OldKeysoundStreamReader
{
    private readonly IVagStreamReader _vagStreamReader;

    public BeatmaniaPs2OldKeysoundStreamReader(IVagStreamReader vagStreamReader)
    {
        _vagStreamReader = vagStreamReader;
    }

    public BeatmaniaPs2KeysoundSet Read(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var header = Enumerable.Range(0, 8).Select(_ => reader.ReadInt32()).ToArray();
        var length = header[0];
        var hunk = reader.ReadBytes(length - 0x20);

        using var hunkMem = new MemoryStream(hunk);
        var hunkReader = new BinaryReader(hunkMem);
        var entries = Enumerable.Range(0, 511).Select(i =>
        {
            hunkMem.Position = i * 0x20;

            var result = new BeatmaniaPs2Keysound
            {
                SampleNumber = hunkReader.ReadInt16(),
                Reserved0 = hunkReader.ReadInt16(),
                Channel = hunkReader.ReadByte(),
                Volume = hunkReader.ReadByte(),
                Panning = hunkReader.ReadByte(),
                SampleType = hunkReader.ReadByte(),
                FrequencyLeft = hunkReader.ReadInt32(),
                FrequencyRight = hunkReader.ReadInt32(),
                OffsetLeft = hunkReader.ReadInt32(),
                OffsetRight = hunkReader.ReadInt32(),
                PseudoLeft = hunkReader.ReadInt32(),
                PseudoRight = hunkReader.ReadInt32()
            };

            switch (result.SampleType)
            {
                case 2:
                {
                    hunkMem.Position = result.OffsetLeft - 61488;
                    result.Data = new List<VagChunk> {_vagStreamReader.Read(hunkMem, 1, 0)};
                    break;
                }
                case 3:
                {
                    hunkMem.Position = header[1] + result.OffsetLeft + 16368;
                    result.Data = new List<VagChunk> {_vagStreamReader.Read(hunkMem, 1, 0)};
                    break;
                }
                case 4:
                {
                    hunkMem.Position = result.OffsetLeft - 61488;
                    var dataLeft = _vagStreamReader.Read(hunkMem, 1, 0);
                    hunkMem.Position = result.OffsetRight - 61488;
                    var dataRight = _vagStreamReader.Read(hunkMem, 1, 0);
                    result.Data = new List<VagChunk> {dataLeft, dataRight};
                    break;
                }
                default:
                {
                    return null;
                }
            }

            return result;
        }).Where(entry => entry != null).ToList();

        return new BeatmaniaPs2KeysoundSet
        {
            Keysounds = entries
        };
    }
}