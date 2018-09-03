using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Ddr.Models;
using RhythmCodex.Infrastructure;

namespace RhythmCodex.Ddr.Converters
{
    [Service]
    public class Ddr573DirectoryDecoder : IDdr573DirectoryDecoder
    {
        private const int PsxIdString = 0x582D5350;

        public IList<Ddr573DirectoryEntry> Decode(Ddr573Image image)
        {
            return DecodeInternal(image).ToList();
        }
        
        private static IEnumerable<Ddr573DirectoryEntry> DecodeInternal(Ddr573Image image)
        {
            if (!image.Modules.Any())
                throw new RhythmCodexException("There must be at least one module in the image.");
            if (!image.Modules.ContainsKey(0))
                throw new RhythmCodexException("A module with index 0 must be present in the image.");

            using (var stream = new MemoryStream(image.Modules[0]))
            using (var reader = new BinaryReader(stream))
            {
                Ddr573DirectoryEntry GetDdrFile()
                {
                    var file = new Ddr573DirectoryEntry
                    {
                        Id = reader.ReadInt32(),
                        Offset = reader.ReadInt16() * 0x800,
                        Module = reader.ReadInt16(),
                        CompressionType = reader.ReadByte(),
                        Reserved0 = reader.ReadByte(),
                        Reserved1 = reader.ReadByte(),
                        Reserved2 = reader.ReadByte(),
                        Length = reader.ReadInt32()
                    };

                    return file.Id == 0 ? null : file;
                }
                
                Ddr573DirectoryEntry GetDsFile()
                {
                    throw new RhythmCodexException("Retrieving Dancing Stage directory entries is not yet supported.");
                }
                
                Func<Ddr573DirectoryEntry> GetDirectoryReader()
                {
                    stream.Position = 0x24;
                    if (reader.ReadInt32() == PsxIdString)
                    {
                        stream.Position = 0xFE4000;
                        if (reader.ReadInt32() > 0)
                        {
                            stream.Position = 0xFE4000;
                            return GetDdrFile;
                        }
                        stream.Position = 0x100000;
                        if (reader.ReadInt32() > 0)
                        {
                            stream.Position = 0x100000;
                            return GetDdrFile;
                        }
                        stream.Position = 0x64F000;
                        if (reader.ReadInt32() > 0)
                        {
                            stream.Position = 0x64F000;
                            return GetDsFile;
                        }
                    }
                    else
                    {
                        stream.Position = 0x824;
                        if (reader.ReadInt32() == PsxIdString)
                        {
                            stream.Position = 0x100800;
                            if (reader.ReadInt32() > 0)
                            {
                                stream.Position = 0x100800;
                                return GetDsFile;
                            }
                        }
                    }

                    throw new RhythmCodexException("The directory is not recognized.");
                }

                var read = GetDirectoryReader();
                while (true)
                {
                    var entry = read();
                    if (entry == null)
                        yield break;
                    yield return entry;
                }
            }
        }
    }
}