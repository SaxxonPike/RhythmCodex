using System.IO;
using RhythmCodex.Beatmania.Models;
using RhythmCodex.Extensions;
using RhythmCodex.IoC;
using RhythmCodex.Vag.Streamers;

namespace RhythmCodex.Beatmania.Streamers;

[Service]
public class BeatmaniaPs2OldBgmStreamReader : IBeatmaniaPs2OldBgmStreamReader
{
    private readonly IVagStreamReader _vagStreamReader;

    public BeatmaniaPs2OldBgmStreamReader(IVagStreamReader vagStreamReader)
    {
        _vagStreamReader = vagStreamReader;
    }
        
    public BeatmaniaPs2Bgm Read(Stream stream)
    {
        var reader = new BinaryReader(stream);
        var length = reader.ReadInt32S();
        reader.ReadByte();
        var volume = reader.ReadByte();
        var rate = reader.ReadUInt16S();
        var channels = reader.ReadByte();
            
        // skip the rest of the header
        reader.Skip(0x800 - 9);

        var source = reader.ReadBytes(length);
        using var mem = new MemoryStream(source);
        var data = _vagStreamReader.Read(mem, channels, 0x800);
        return new BeatmaniaPs2Bgm
        {
            Data = data,
            Volume = volume,
            Rate = rate,
        };
    }
}