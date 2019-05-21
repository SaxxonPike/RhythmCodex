using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Psf.Models;

namespace RhythmCodex.Psf.Streamers
{
    [Service]
    public class PsfStreamReader : IPsfStreamReader
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
            
            using (var inputMemory = new MemoryStream(dataCompressed))
            using (var deflate = new DeflateStream(inputMemory, CompressionMode.Decompress))
            using (var outputMemory = new MemoryStream())
            {
                deflate.CopyTo(outputMemory);
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
}