using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Streamers;

[Service]
public class XboxHbnStreamReader : IXboxHbnStreamReader
{
    public IEnumerable<XboxHbnEntry> Read(Stream hbnStream, Stream binStream)
    {
        var binBaseOffset = binStream.Position;
        var hbnReader = new StreamReader(hbnStream);
            
        if (hbnReader.ReadLine() != "Konami Computer Entertainment Hawaii, Inc.")
            throw new RhythmCodexException("Unrecognized HBN.");
            
        if (hbnReader.ReadLine() != "!! This file is auto-generated by x_bincat.  DO NOT MANUALLY EDIT .HBN's !!")
            throw new RhythmCodexException("Unrecognized HBN.");

        var count = int.Parse(hbnReader.ReadLine());
        var entries = Enumerable
            .Range(0, count)
            .Select(_ =>
            {
                var definition = hbnReader.ReadLine().Split(',');
                return new XboxHbnDefinition
                {
                    Name = definition[0],
                    Offset = int.Parse(definition[1]),
                    Length = int.Parse(definition[2])
                };
            })
            .ToArray();

        return entries
            .Select(e =>
            {
                binStream.Position = binBaseOffset + e.Offset;
                    
                var reader = new BinaryReader(binStream);
                return new XboxHbnEntry
                {
                    Name = e.Name,
                    Data = reader.ReadBytes(e.Length)
                };
            });
    }
}