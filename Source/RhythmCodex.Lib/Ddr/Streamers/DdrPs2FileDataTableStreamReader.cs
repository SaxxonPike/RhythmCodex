using System;
using System.IO;
using System.Linq;
using RhythmCodex.Compression;
using RhythmCodex.Infrastructure;
using RhythmCodex.IoC;

namespace RhythmCodex.Ddr.Streamers
{
    [Service]
    public class DdrPs2FileDataTableStreamReader : IDdrPs2FileDataTableStreamReader
    {
        private readonly IBemaniLzDecoder _bemaniLzDecoder;

        public DdrPs2FileDataTableStreamReader(IBemaniLzDecoder bemaniLzDecoder)
        {
            _bemaniLzDecoder = bemaniLzDecoder;
        }

        public byte[] Get(Stream stream)
        {
            using var snapshot = new SnapshotStream(stream);
            var reader = new BinaryReader(snapshot);
            var firstFile = reader.ReadInt32();
            var tableSize = firstFile / 4 * 4;
            var table = Enumerable.Range(0, firstFile / 4).Select(_ => reader.ReadInt32()).ToArray();
            var skip = firstFile - tableSize;

            if (skip > 0)
                reader.ReadBytes(skip);

            for (var i = 1; i < table.Length; i++)
                if (table[i] < table[i - 1])
                    throw new Exception("Invalid table");

            reader.ReadBytes(table.Last() - table[0]);
            try
            {
                _bemaniLzDecoder.Decode(snapshot);
            }
            catch (Exception)
            {
                return null;
            }
            return snapshot.ToArray();
        }
    }
}