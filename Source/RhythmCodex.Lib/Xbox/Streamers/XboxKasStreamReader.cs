using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RhythmCodex.Extensions;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;
using RhythmCodex.Xbox.Model;

namespace RhythmCodex.Xbox.Streamers
{
    // TODO: make this work on forward-only streams
    
    [Service]
    public class XboxKasStreamReader : IXboxKasStreamReader
    {
        public IEnumerable<XboxKasEntry> Read(Stream kasStream)
        {
            var baseOffset = kasStream.Position;
            var reader = new BinaryReader(kasStream);

            var count = reader.ReadInt32S();
            if (count < 0)
                throw new RhythmCodexException("Invalid entry count.");

            var blockTable = new int[count];
            var lengthTable = new int[count];
            var offsetTable = new int[count];

            for (var i = 0; i < count; i++)
                blockTable[i] = reader.ReadInt32S();
            for (var i = 0; i < count; i++)
                lengthTable[i] = reader.ReadInt32S();
            for (var i = 0; i < count; i++)
                offsetTable[i] = reader.ReadInt32S();

            for (var i = 0; i < count; i++)
            {
                // skip what we think are bad entries
                if (lengthTable[i] <= 0 || offsetTable[i] <= count * 12 || blockTable[i] <= 0)
                    continue;
                
                kasStream.Position = baseOffset + offsetTable[i];
                var data = new byte[lengthTable[i]];
                var actualRead = kasStream.TryRead(data, 0, lengthTable[i]);
                if (actualRead < data.Length)
                    Array.Resize(ref data, actualRead);

                yield return new XboxKasEntry
                {
                    Block = blockTable[i],
                    Offset = offsetTable[i],
                    Data = data
                };
            }
        }
    }
}