using System.IO;
using RhythmCodex.Compressions.Zlib.Processors;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Sounds.Psf.Models;

namespace RhythmCodex.Sounds.Psf.Streamers;

[Service]
public class PsfStreamReader(IZlibStreamFactory zlibStreamFactory) : IPsfStreamReader
{
    public PsfChunk Read(Stream source)
    {
        var reader = new BinaryReader(source);
        var id = reader.ReadInt32();
            
        if ((id & 0x00FFFFFF) != 0x00465350)
            throw new RhythmCodexException($"Bad PSF ID. Found: {id:X8}");

        var version = (id >> 24) & 0xFF;
        var reservedSize = reader.ReadInt32();
        var dataSize = reader.ReadInt32();
        var crc = reader.ReadInt32();

        var reserved = reader.ReadBytes(reservedSize);
        var dataCompressed = reader.ReadBytes(dataSize);
        byte[] data;
            
        using (var inputMemory = zlibStreamFactory.Create(new MemoryStream(dataCompressed)))
        using (var outputMemory = new MemoryStream())
        {
            inputMemory.CopyTo(outputMemory);
            data = outputMemory.ToArray();
        }
            
        return new PsfChunk
        {
            Crc = crc,
            Version = version,
            Reserved = reserved,
            Data = data
        };
    }
}